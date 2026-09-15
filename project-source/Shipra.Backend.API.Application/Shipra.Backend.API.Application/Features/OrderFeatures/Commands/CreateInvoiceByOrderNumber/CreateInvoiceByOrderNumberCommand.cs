using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateInvoiceByOrderNumber;
public class CreateInvoiceByOrderNumberCommand : IRequest<ServiceResultDTO>
{
  public string? OrderNo { get; set; }
}
public class CreateInvoiceByOrderNumberCommandHandler : RequestHandlerBase<CreateInvoiceByOrderNumberCommand, ServiceResultDTO>
{
  private readonly IStripeRepository _stripeRepository;
  private readonly IClientRepository _clientRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly IPaymentProcessRepository _paymentProcessRepository;

  public CreateInvoiceByOrderNumberCommandHandler(IStripeRepository stripeRepository, IClientRepository clientRepository, IServiceProvider serviceProvider, IOrderRepository orderRepository, IPaymentProcessRepository paymentProcessRepository, ILogger<CreateInvoiceByOrderNumberCommandHandler> logger) : base(serviceProvider, logger)
  {
    _stripeRepository = stripeRepository;
    _clientRepository = clientRepository;
    _orderRepository = orderRepository;
    _paymentProcessRepository = paymentProcessRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateInvoiceByOrderNumberCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var client = await _clientRepository.GetClientById(_currentUser.ClientId!);
      if (client is null)
      {
        throw new EntityNotFoundException("Client ", _currentUser.ClientIdStr!);
      }

      var order = await _orderRepository.GetOrderByOrderNo(request.OrderNo!, _currentUser.ClientId!);
      if (order == null)
      {
        throw new EntityNotFoundException("Order ", request.OrderNo!);
      }

      if (order.CarrierId is null)
      {
        //Success: Invoice will be generated when order not assign to a carrier
        var oItems = await _orderRepository.GetOrderItemsByOrderId(order.OrderId);
        var oOrderAddress = await _orderRepository.GetOrderAddressById(order.OrderAddressId!.GetValueOrDefault());
        var oPaymentProcess = await _paymentProcessRepository.GetClientDefaultPaymentProcessById(_currentUser.ClientId!);
        if (oPaymentProcess is not null)
        {
          #region Stripe Payment Process
          //The Below region is for Stripe Payment
          if (oPaymentProcess.PplookupId == (int)EnumPaymentProcessLookup.Stripe)
          {
            //Deserialize Stripe Config values
            var stripeSettingJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(oPaymentProcess.Config!);

            //Convert Keys To CamelCase with Deserialize Stripe object
            var resultStripeSetting = Utils.ConvertKeysToCamelCase(stripeSettingJson!);

            //Stripe keys
            var secretKey = Utils.GetValueFromDictionryByKey("secretKey", resultStripeSetting);
            var publicKey = Utils.GetValueFromDictionryByKey("publicKey", resultStripeSetting);

            if (oOrderAddress is not null)
            {
              //Success: The Order Address to get customer information
              if (!string.IsNullOrEmpty(secretKey))
              {
                var accountStripe = await _stripeRepository.ValidateStripeAccount(secretKey);
                if (!string.IsNullOrEmpty(accountStripe))
                {
                  var customerId = await _stripeRepository.CreateCustomerAsync(oOrderAddress.CustomerName!, oOrderAddress.Mobile1!, oOrderAddress.Email, secretKey);

                  if (!string.IsNullOrEmpty(customerId))
                  {
                    var invoiceResult = await _stripeRepository.CreateInvoice(order, oItems, customerId!, secretKey);

                    order.UpdateOrderByStripeInvoiceInformation(customerId, invoiceResult.HostedInvoiceUrl, invoiceResult.InvoicePdf, invoiceResult.Id);
                    await _orderRepository.UpdateOrder(order);

                    serviceResult = new ServiceResultDTO(new { order.OrderNo,invoiceResult.HostedInvoiceUrl, invoiceResult.InvoicePdf });
                  }
                  else
                  {
                    throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "Request failed while creating customer on stripe.");
                  }
                }
                else
                {
                  throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "Request failed due to invalid stripe secret key.");
                }
              }
              else
              {
                throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "Request failed due to stripe secret key maybe null or empty.");
              }
            }
            else
            {
              throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "Request failed while getting Order Address details.");
            }
          }
          #endregion
        }
        else
        {
          throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "Request failed while getting Default payment process setting details.");
        }
      }
      else
      {
        throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "Request failed due to the order is already assigned to a carrier.");
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
