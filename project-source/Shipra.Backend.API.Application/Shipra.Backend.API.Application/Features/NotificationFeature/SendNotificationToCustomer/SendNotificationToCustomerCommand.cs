using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetOrderInvoiceByOrderNo;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.NotificationFeature.SendEmailToCustomer;
public class SendNotificationToCustomerCommand : IRequest<ServiceResultDTO>
{
  public string? OrderNo { get; set; }
  public int? NotificationTypeId { get; set; }
}
public class SendEmailToCustomerCommandHandler : RequestHandlerBase<SendNotificationToCustomerCommand, ServiceResultDTO>
{
  private readonly IOrderRepository _orderRepository; 
  private readonly IMediator _mediator;
  public SendEmailToCustomerCommandHandler(IOrderRepository orderRepository,IMediator mediator, IServiceProvider serviceProvider, ILogger<SendEmailToCustomerCommandHandler> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository; 
    _mediator = mediator;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(SendNotificationToCustomerCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var order = await _orderRepository.GetOrderByOrderNo(request.OrderNo!, _currentUser.ClientId!);
      if (order != null)
      {
        string subject = string.Empty;
        var orderAdrress = await _orderRepository.GetOrderAddressById(order.OrderAddressId!.GetValueOrDefault());
        if (orderAdrress != null)
        {
          if (request.NotificationTypeId == (int)EnumNotificationReminderTypes.SendPayment)
          {
            if (order.PaymentMethodId != (int)EnumPaymentMethod.PP && order.PaymentStatusId != (int)EnumPaymentStatus.Paid)
            {
              if (order.StripeInvoiceHostURL is not null)
              {
                var body = GetSendPaymentBody(order.StripeInvoiceHostURL!);
                subject = "Payment LinkWooComereceModal";
                var result = await _emailServiceProvider.SendEmailToCustomer(orderAdrress?.Email, body, subject);
                if (result)
                {
                  serviceResult = new ServiceResultDTO(new { Message = "Email send successfully!" });
                }
                else
                {
                  serviceResult = new ServiceResultDTO(new
                  {
                    Message = "StripeInvoiceHostURL does not exist "
                  });
                }
              }
            }

          }
          else if (request.NotificationTypeId == (int)EnumNotificationReminderTypes.SendShipping)
          {
            var body = GetSendPaymentBody(orderAdrress!.CustomerName, orderAdrress.Email, null);
            subject = "Shipping LinkWooComereceModal";
            var result = await _emailServiceProvider.SendEmailToCustomer(orderAdrress?.Email, body, subject, "{ { baseurl} }/ api / Order / GetOrderTrackingHistoryByOrderNo ? OrderNo ={request.OrderNo}");
            if (result)
            {
              serviceResult = new ServiceResultDTO(new { Message = "Email send successfully!" });
            }
          }
          else if (request.NotificationTypeId == (int)EnumNotificationReminderTypes.SendInvoice)
          {
            var orderInvoice = new GetOrderInvoiceByOrderNoQuery()
            {
              OrderNos = request.OrderNo
            };
            var result = await _mediator.Send(orderInvoice, cancellationToken);
            var attachmentFile = result.Result;
            if (order.PaymentStatusId != (int)EnumPaymentStatus.Paid)
            {
              var body = GetSendPaymentBody(orderAdrress!.CustomerName, orderAdrress.Email, null);
              subject = "Invoice LinkWooComereceModal";
              var flag = await _emailServiceProvider.SendEmailToCustomer(orderAdrress?.Email, body, subject, null, attachmentFile, $"Order Invoice - Order No:{request.OrderNo}");
              if (flag)
              {
                serviceResult = new ServiceResultDTO(new { Message = "Email send successfully!" });
              }
            }
          }
        }
      }
      else
      {
        serviceResult.IsSuccess = false;
        serviceResult.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
        serviceResult.Errors!.Add("Error", new string[] { "Something went wrong!" });
      }
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
  private string GetSendPaymentBody(string? name = null, string? email = null, string? stripeInvoiceHostURL = null)
  {
    string body;
    if (stripeInvoiceHostURL is not null)
    {
      body = "<div style='line-height:inherit;font-family:Avenir,Helvetica,sans-serif;box-sizing:border-box;direction:ltr;text-align:left;'>";
      body += "<br style='line-height:inherit;'><b style='font-size: 16px;'>Contact Information</b>";
      body += "<br style='line-height:inherit;'><b>Stripe LinkWooComereceModal:</b> " + stripeInvoiceHostURL;
      body += "<br style='line-height: inherit;'>";
      body += "<b>Shipra</b> App <br style = 'line-height: inherit;' >";
      body += "<b>Note:</b> This is an electronic message.Please do not reply to this email.</div>";
    }
    else
    {
      body = "<div style='line-height:inherit;font-family:Avenir,Helvetica,sans-serif;box-sizing:border-box;direction:ltr;text-align:left;'>";
      body += "<br style='line-height:inherit;'><b style='font-size: 16px;'>Contact Information</b>";
      body += "<br style='line-height:inherit;'><b>Name:</b> " + name;
      body += "<br style='line-height:inherit;'><b>Email Address:</b> " + email;
      body += "<br style='line-height: inherit;'>";
      body += "<b>Shipra</b> App <br style = 'line-height: inherit;' >";
      body += "<b>Note:</b> This is an electronic message.Please do not reply to this email.</div>";
    }
    return body;
  }
}
