using System.Net;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.CarrierUseCase;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.WalletAggregate;
using Shipra.Backend.API.SharedKernel.Interfaces;
using Shipra.Backend.API.SharedKernel.Models;

namespace Shipra.Backend.API.Application.Features.TotalProcessFeature.Command.GeneratePaymentLink;
public class GenerateTotalProcessPaymentLinkCommand : IRequest<ServiceResultDTO>
{
  public string? OrderNo { get; set; }
}
public class GenerateTotalProcessPaymentLinkCommandHandler : RequestHandlerBase<GenerateTotalProcessPaymentLinkCommand, ServiceResultDTO>
{
  private readonly IMasterDbRepository _masterDbRepository;
  private readonly IWalletRepository _walletRepository;
  private readonly ISharedTotalProcessingRepository _sharedTotalProcessingRepository;
  private readonly IConfigRepository _configRepository;
  private readonly IOrderRepository _orderRepository;

  public GenerateTotalProcessPaymentLinkCommandHandler(IMasterDbRepository masterDbRepository, IWalletRepository walletRepository, ISharedTotalProcessingRepository sharedTotalProcessingRepository, IConfigRepository configRepository, IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<GenerateTotalProcessPaymentLinkCommandHandler> logger) : base(serviceProvider, logger)
  {
    _masterDbRepository = masterDbRepository;
    _walletRepository = walletRepository;
    _sharedTotalProcessingRepository = sharedTotalProcessingRepository;
    _configRepository = configRepository;
    _orderRepository = orderRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(GenerateTotalProcessPaymentLinkCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var order = await _orderRepository.GetOrderByOrderNo(request.OrderNo!, _currentUser.ClientId!);
      if (order == null)
      {
        throw new EntityNotFoundException("Order ", request.OrderNo!);
      } 

      if (order.PaymentMethodId == (int)EnumPaymentMethod.PP)
      {
        throw new Exception("Payment link cannot be generated for prepaid orders.");
      } 
      if (order.Amount > 5000)
      {
        throw new Exception("Please contact Shipra support; the payment link amount cannot exceed 5000.");
      }

      OrderId orderId = order!.OrderId!;
      PaymentLink? oPaymentLink = await _walletRepository.GetPaymentLinkByOrderId(orderId);
      if (oPaymentLink is not null)
      {
        serviceResult.IsSuccess = false;
        serviceResult.Errors!.Add("AlreadyExist", new string[] { $"Payment link already exist against this order:{request.OrderNo}" });
        return serviceResult;
      }

      if (oPaymentLink is null)
      {
        if (order.Amount.GetValueOrDefault() == 0)
        {
          serviceResult.IsSuccess = false;
          serviceResult.Errors!.Add("AmmountEqual0", new string[] { $"You cannot create a payment link for an order with a zero amount." });
          return serviceResult;
        }

        if (order.CarrierId is null)
        {
          int expiry_date_days = 7;
          //Success: Invoice will be generated when order not assign to a carrier
          var oItems = await _orderRepository.GetOrderItemsByOrderId(order.OrderId);
          var oOrderAddress = await _orderRepository.GetOrderAddressById(order.OrderAddressId!.GetValueOrDefault());

          var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.Admin, _currentUser.EnvironmentTypeId);
          if (mcconfig is null)
          {
            throw new EntityNotFoundException("Mcconfig", "Integration Value");
          }
          #region total processing

          if (oOrderAddress is not null)
          {
            var catalog = await _masterDbRepository.GetCatalogueByClientId(_currentUser.ClientIdStr!);
            if (catalog is not null)
            {
              string sender = oOrderAddress!.CustomerName!.Length > 11 ? oOrderAddress.CustomerName.Substring(0, 11) : oOrderAddress.CustomerName;
              TotalProcessingGeneratePaymentLinkRequest reqModel = new TotalProcessingGeneratePaymentLinkRequest()
              {
                reference = $"{catalog.CatalogueId}-{order.OrderNo}",
                contact_name = oOrderAddress.CustomerName,
                sender = sender,
                Email = oOrderAddress?.Email,
                Amount = order.Amount.GetValueOrDefault(),
                Currency = "AED",
                auto_send_email = false,
                auto_send_sms = false,
                expiry_date = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(expiry_date_days),
                payment_type = "PA",
                OrderNo = order.OrderNo,
                ClientId = _currentUser.ClientIdStr!,
              };

              var requestResponse = await _sharedTotalProcessingRepository.GenerateTotalProcessingPaymentLink(reqModel, mcconfig.Value!);
              if (!string.IsNullOrEmpty(requestResponse))
              {
                var result = JsonConvert.DeserializeObject<ShipraControlPaneResponseModel<GeneratePaymentLinkResponse>>(requestResponse);
                if (result != null && result!.isSuccess)
                {
                  //Success: The Order Address to get customer information

                  #region create payment link
                  PaymentLink paymentLink = PaymentLink.CreatePaymentLink(result.result?.Link, result!.result?.Uuid, null, orderId, (int)EnumPaymentLinkStatus.Unpaid, _currentUser.ClientId!,order.Amount.GetValueOrDefault());
                  bool isCreated = await _walletRepository.CreatePaymentLink(paymentLink);
                  #endregion

                  serviceResult = new ServiceResultDTO(new { order.OrderNo, result.result?.Link, result.result?.Uuid });

                }
                else
                {
                  serviceResult.Errors = result?.errors;
                  serviceResult.IsSuccess = false;
                }
              }
              else
              {
                serviceResult.IsSuccess = false;
                serviceResult.Errors!.Add("ThirdPartyError", new string[] { "Error while create link" });
              }
            }
            else
            {
              throw new EntityNotFoundException("Catalog", _currentUser.ClientIdStr!);

            }
          }
          else
          {
            throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "Request failed while getting Order Address details.");
          }
          #endregion

        }
        else
        {
          throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "Request failed due to the order is already assigned to a carrier.");
        }
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

public class GenerateTotalProcessPaymentLinkCommandValidator : AbstractValidator<GenerateTotalProcessPaymentLinkCommand>
{
  public GenerateTotalProcessPaymentLinkCommandValidator()
  {
    RuleFor(x => x.OrderNo).NotEmpty().NotNull();
  }
}
