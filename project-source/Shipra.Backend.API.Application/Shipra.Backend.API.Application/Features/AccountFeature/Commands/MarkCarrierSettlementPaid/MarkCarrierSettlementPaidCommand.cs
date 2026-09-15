using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Base.Response;
using Shipra.Backend.API.Core.AccountAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;
using Microsoft.Extensions.DependencyInjection;
using Shipra.Backend.API.Core.Helper;

namespace Shipra.Backend.API.Application.Features.AccountFeature.Commands.MarkCarrierSettlementPaid;
public class MarkCarrierSettlementPaidCommand : IRequest<ServiceResultDTO>
{
  public string? CarrierPaymentSettlementId { get; set; }
}
public class MarkCarrierSettlementPaidCommandHandler : RequestHandlerBase<MarkCarrierSettlementPaidCommand, ServiceResultDTO>
{
  private readonly IOrderTrackingHistoryRepository _orderTrackingHistoryRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly IAccountRepository _accountRepository;
  private readonly IClientRepository _clientRepository;

  public MarkCarrierSettlementPaidCommandHandler(IOrderTrackingHistoryRepository orderTrackingHistoryRepository, IOrderRepository orderRepository, IAccountRepository accountRepository, IClientRepository clientRepository, IServiceProvider serviceProvider, ILogger<MarkCarrierSettlementPaidCommandHandler> logger) : base(serviceProvider, logger)
  {
    _orderTrackingHistoryRepository = orderTrackingHistoryRepository;
    _orderRepository = orderRepository;
    _accountRepository = accountRepository;
    _clientRepository = clientRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(MarkCarrierSettlementPaidCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var carrierPaymentSettlementId = new CarrierPaymentSettlementId(new Guid(request.CarrierPaymentSettlementId!));
      var oCarrierPaymentSettlement = await _accountRepository.GetCarrierPaymentSettlementById(carrierPaymentSettlementId);

      if (oCarrierPaymentSettlement != null)
      {
        oCarrierPaymentSettlement!.MarkCarrierSettlementPaid(_currentUser.EmployeeId!);
        var oResult = await _accountRepository.UpdateCarrierPaymentSettlement(oCarrierPaymentSettlement);

        #region update order status to paid and lock order
        var clientSetting = await _clientRepository.GetGenericSettingByClientIdAsync(_currentUser.ClientId!);
        bool trackingLock = true;
        if (clientSetting != null && !string.IsNullOrEmpty(clientSetting.SettingConfig))
        {
          var val = UtilityHelper.GetClientSettingValueWithByKey(clientSetting.SettingConfig, "others", "trackingLock");
          if (!string.IsNullOrEmpty(val))
          {
            trackingLock = UtilityHelper.GetBoolFromString(val);
          }
        }

        var oOrders = await _orderRepository.GetAllOrdersByPaymentSettlementId(carrierPaymentSettlementId);
        foreach (var oOrder in oOrders)
        {
          oOrder.MarkStatusPaidAndLockOrder(_currentUser.EmployeeId, trackingLock);
          await _orderRepository.UpdateOrder(oOrder);
          //create order note
          var oOrderTrackingHistory = await _orderTrackingHistoryRepository.CreateOrderNote(OrderNote.CreateOrderNote(oOrder!.OrderId!, "Carrier settlement mark paid against " + oCarrierPaymentSettlement.PaymentRef, _currentUser.EmployeeId!));
        }
        #endregion

        serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = request.CarrierPaymentSettlementId, Message = "Settlement Paid successfully." });
        return serviceResult;
      }
      else
      {
        serviceResult.CreateError("NotFound", new string[] { "Carrier settlement not found" });
        return serviceResult;
      }

    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class MarkCarrierSettlementPaidCommandValidator : AbstractValidator<MarkCarrierSettlementPaidCommand>
{
  public MarkCarrierSettlementPaidCommandValidator()
  {
    RuleFor(x => x.CarrierPaymentSettlementId).NotEmpty().NotEmpty().Must(GuidHelper.Validator).WithMessage(GuidHelper.GuidMessage); ; 
  }
}
