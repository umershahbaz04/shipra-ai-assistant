using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.EMMA;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.CarrierUseCase;
using Shipra.Backend.API.Application.Features.CarrierFeatures.Query.GetAwbForCarrierByOrderNos;
using Shipra.Backend.API.Application.Features.WalletFeature.Command.UpdateWalletWithTotalProcessingLinkData;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.SharedKernel.Interfaces;
using Shipra.Backend.API.SharedKernel.Models;

namespace Shipra.Backend.API.Application.Features.TotalProcessFeature.Query.FetchPaybylinkbyServiceUUId;
public class FetchPaybylinkbyServiceUUIdQuery : IRequest<ServiceResultDTO>
{
  public string? PaymentLinkId { get; set; }
}
public class FetchPaybylinkbyServiceUUIdQueryHandler : RequestHandlerBase<FetchPaybylinkbyServiceUUIdQuery, ServiceResultDTO>
{
  private readonly IMediator _mediator;
  private readonly IOrderRepository _orderRepository;
  private readonly IWalletRepository _walletRepository;
  private readonly IConfigRepository _configRepository;
  private readonly ISharedTotalProcessingRepository _sharedTotalProcessingRepository;

  public FetchPaybylinkbyServiceUUIdQueryHandler(IMediator mediator, IOrderRepository orderRepository, IWalletRepository walletRepository, IConfigRepository configRepository, ISharedTotalProcessingRepository sharedTotalProcessingRepository, IServiceProvider serviceProvider, ILogger<FetchPaybylinkbyServiceUUIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _mediator = mediator;
    _orderRepository = orderRepository;
    _walletRepository = walletRepository;
    _configRepository = configRepository;
    _sharedTotalProcessingRepository = sharedTotalProcessingRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(FetchPaybylinkbyServiceUUIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var oPaymentLink = await _walletRepository.GetPaymentLinkByPaymentLinkId(new Core.WalletAggregate.PaymentLinkId(new Guid(request.PaymentLinkId!)));
      if (oPaymentLink is null)
      {
        throw new EntityNotFoundException("PaymentLink", request.PaymentLinkId!);
      }

      if (oPaymentLink.PaymentLinkStatusId == (int)EnumPaymentStatus.Paid)
      {
        serviceResult.CreateError("AlreadyPaid", new string[] { $"The payment link associated with the given ID:{request.PaymentLinkId} has already been paid." });
        return serviceResult;
      }

      var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.Admin, _currentUser.EnvironmentTypeId);
      if (mcconfig is null)
      {
        throw new EntityNotFoundException("Mcconfig", "Integration Value");
      }

      string requestResponse = await _sharedTotalProcessingRepository.FetchPaybylinkbyServiceUUID(oPaymentLink.ServiceUUId, _currentUser.ClientIdStr, mcconfig.Value!);
      if (!string.IsNullOrEmpty(requestResponse))
      {
        var result = JsonConvert.DeserializeObject<ShipraControlPaneResponseModel<PayByLinkDetailsResponseModel>>(requestResponse);
        if (result != null && result!.isSuccess)
        {
          //Success: The Order Address to get customer information
          if (result!.result is not null && result!.result!.Paid)
          {
            var oOrder = await _orderRepository.GetOrderById(oPaymentLink.OrderId!, _currentUser.ClientId!);
            if (oOrder is null)
            {
              throw new EntityNotFoundException("Order ", oPaymentLink.OrderId!);
            }

            UpdateWalletWithTotalProcessingLinkDataCommand updateWallet = new UpdateWalletWithTotalProcessingLinkDataCommand()
            {
              OrderNo = oOrder.OrderNo,
              Ammount = oOrder.Amount.GetValueOrDefault() 
            };
            serviceResult = await _mediator.Send(updateWallet, cancellationToken);
          }
          else
          {
            serviceResult.CreateError("StatusIsNotPaid", new string[] { "The payment has not been completed." });
          }

          //serviceResult = new ServiceResultDTO(result.result!);

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
        serviceResult.Errors!.Add("ThirdPartyError", new string[] { "Error while fetch status" });
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
public class FetchPaybylinkbyServiceUUIdQueryValidator : AbstractValidator<FetchPaybylinkbyServiceUUIdQuery>
{
  public FetchPaybylinkbyServiceUUIdQueryValidator()
  {
    RuleFor(x => x.PaymentLinkId).NotEmpty().NotNull().Must(GuidHelper.Validator).WithMessage(GuidHelper.GuidMessage);
  }
}
