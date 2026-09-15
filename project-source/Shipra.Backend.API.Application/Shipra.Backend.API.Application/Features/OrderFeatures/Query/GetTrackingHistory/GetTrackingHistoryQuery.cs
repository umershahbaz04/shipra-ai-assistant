using System.Dynamic;
using Ardalis.Result;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ClientUseCase.Response;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.WalletAggregate;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetTrackingHistory;
public class GetTrackingHistoryQuery : IRequest<ServiceResultDTO>
{
  public string? ApiKey { get; set; }
}
public class GetTrackingHistoryQueryHandler : RequestHandlerBase<GetTrackingHistoryQuery, ServiceResultDTO>
{
  private readonly IReturnRepository _returnRepository;
  private readonly IClientRepository _clientRepository;
  private readonly IKeyGeneratorService _keyGeneratorService;
  private readonly IOrderTrackingHistoryRepository _orderTrackingHistoryRepository;

  public GetTrackingHistoryQueryHandler(IReturnRepository returnRepository, IClientRepository clientRepository, IKeyGeneratorService keyGeneratorService, IOrderTrackingHistoryRepository orderTrackingHistoryRepository, IServiceProvider serviceProvider, ILogger<GetTrackingHistoryQueryHandler> logger) : base(serviceProvider, logger)
  {
    _returnRepository = returnRepository;
    _clientRepository = clientRepository;
    _keyGeneratorService = keyGeneratorService;
    _orderTrackingHistoryRepository = orderTrackingHistoryRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetTrackingHistoryQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var requestData = request!.ApiKey?.Split("_");
      dynamic result = new ExpandoObject();
      result.clientCompanyName = string.Empty;
      result.companyImage = string.Empty;
      result.returnOrder = null;
      result.history = new List<dynamic>();
      if (requestData is not null)
      {

        if (requestData.Count() >= 2)
        {
          var clientId = requestData.FirstOrDefault();
          string? orderNo = requestData?.Length switch
          {
            2 => requestData.LastOrDefault(), // When length is 2, take the last element
            3 => string.Join("_", requestData.Skip(requestData.Length - 2)), // When length is 3, take the last two elements
            _ => null // Handle other cases if needed
          };

          #region order
          var oOrder = await _returnRepository.GetOrderByOrderNo(orderNo!, new ClientId(new Guid(clientId!)));

          if (oOrder != null)
          {
            DateTime? paidOn = null;
            string paymentStatus = "UNPAID";
            string paidStatus = "PAID";
            string paymentVia = "";
            //credit card //payment done // prepaid by card cod cash // card (assign carreier then no payment button) 
            if (oOrder.PaymentMethodId == (int)EnumPaymentMethod.PP)
            {
              paidOn = oOrder.CreatedOn;
              paymentStatus = paidStatus;
               
              paymentVia = "via cash";
            }
            else
            {
              paymentVia = "via cod cash"; 
            }
            //get payment info
            PaymentLink? oPaymentLink = await _returnRepository.GetPaymentLinkByOrderId(oOrder.OrderId, new ClientId(new Guid(clientId!)));
            if (oPaymentLink is not null)
            {
              if (oPaymentLink.PaymentLinkStatusId == (int)EnumPaymentLinkStatus.Paid)
              {
                paidOn = oPaymentLink.PaidOn;
                paymentStatus = paidStatus;

                //get payment link
                paymentVia = "via card";
              }
              if (oOrder.PaymentMethodId == (int)EnumPaymentMethod.COD)
              {
                result.paymentLink = oPaymentLink.PaymentLinkUrl;
              }
            };

            if (oOrder.CarrierId != null) //if assigned to carrier then dont show payment link on page
            {
              result.paymentLink = null;
            }
            //else
            //{
            //  //reset to unapid 
            //  paymentStatus = "UNPAID"; 
            //  paymentVia = "via cod cash";
            //}

            result.CarrierTrackingStatus = oOrder.CarrierTrackingStatus;
            result.orderAddress = await _returnRepository.GetStoreAndCustomerAddressByOrderId(oOrder.OrderId!.Value.ToString(), clientId!);
            result.orderItems = await _returnRepository.GetAllOrderItems(oOrder.OrderId!.Value.ToString(), clientId!);
            result.taxInfo = await _returnRepository.GetOrderTaxInfo(oOrder.OrderId!.Value.ToString(), clientId!);
            result.paymentDetail = new
            {
              paymentStatus,
              oOrder.OrderNo,
              currency = "AED",
              oOrder.Amount,
              paymentVia,
              paidOn
            };
          }
          else
          {
            return serviceResult;
          }

          #endregion

          //check return report is already exist then do something
          var returnObj = await _returnRepository.GetReturnReportDataByOrderIdForTracking(orderNo!, clientId!);
          if (returnObj is not null)
          {
            returnObj.OrderItems = result.orderItems;
            result.returnOrder = returnObj;
          }

          if (returnObj!.IsReturnExist.GetValueOrDefault())
          {
            dynamic? data = await _returnRepository.GetReturnTrackingHistory(returnObj.ReturnId, clientId);
            result.history = data;
          }
          else
          {

            var data = await _orderTrackingHistoryRepository.GetOrderTrackingHistoryByOrderNoForView(orderNo!, clientId!);
            result.history = data;
          }


          var client = await _clientRepository.GetClientByIdWithConnectionstring(new ClientId(new Guid(clientId!)));
          if (client is not null)
          {
            result.clientCompanyName = (client != null ? !string.IsNullOrEmpty(client.ClientCompanyName) ?
              client.ClientCompanyName : client.ClientName! : "");
            result.companyImage = (client != null ? client.ClientImage : "");
          }
          serviceResult = new ServiceResultDTO(result);
        }
      }
      else
      {
        serviceResult = new ServiceResultDTO(result);
      }

      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class GetTrackingHistoryQueryValidator : AbstractValidator<GetTrackingHistoryQuery>
{
  public GetTrackingHistoryQueryValidator()
  {
    RuleFor(x => x.ApiKey).NotEmpty().NotEmpty();
  }
}
