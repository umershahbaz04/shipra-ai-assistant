using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.AccountAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.AccountFeature.Commands.MarkCarrierSettlementUnPaid;
public class MarkCarrierSettlementUnPaidCommand : IRequest<ServiceResultDTO>
{
  public string? CarrierPaymentSettlementId { get; set; }
}
public class MarkCarrierSettlementUnPaidCommandHandler : RequestHandlerBase<MarkCarrierSettlementUnPaidCommand, ServiceResultDTO>
{
  private readonly IOrderTrackingHistoryRepository _orderTrackingHistoryRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly IAccountRepository _accountRepository;

  public MarkCarrierSettlementUnPaidCommandHandler(IOrderTrackingHistoryRepository orderTrackingHistoryRepository, IOrderRepository orderRepository, IAccountRepository accountRepository, IServiceProvider serviceProvider, ILogger<MarkCarrierSettlementUnPaidCommandHandler> logger) : base(serviceProvider, logger)
  {
    _orderTrackingHistoryRepository = orderTrackingHistoryRepository;
    _orderRepository = orderRepository;
    _accountRepository = accountRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(MarkCarrierSettlementUnPaidCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var carrierPaymentSettlementId = new CarrierPaymentSettlementId(new Guid(request.CarrierPaymentSettlementId!));
      var oCarrierPaymentSettlement = await _accountRepository.GetCarrierPaymentSettlementById(carrierPaymentSettlementId);

      if (oCarrierPaymentSettlement != null)
      {
        oCarrierPaymentSettlement!.MarkCarrierSettlementUnPaid(_currentUser.EmployeeId!);
        var oResult = await _accountRepository.UpdateCarrierPaymentSettlement(oCarrierPaymentSettlement);

        #region update order status to paid and lock order
        var oOrders = await _orderRepository.GetAllOrdersByPaymentSettlementId(carrierPaymentSettlementId);
        foreach (var oOrder in oOrders)
        {
          oOrder.MarkStatusUnPaidAndUnLockOrder(oOrder.PaymentMethodId,_currentUser.EmployeeId);
          await _orderRepository.UpdateOrder(oOrder);
          //create order note
          var oOrderTrackingHistory = await _orderTrackingHistoryRepository.CreateOrderNote(OrderNote.CreateOrderNote(oOrder!.OrderId!, "Carrier settlement mark Unpaid against " + oCarrierPaymentSettlement.PaymentRef, _currentUser.EmployeeId!));
        }
        #endregion

        serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = request.CarrierPaymentSettlementId, Message = "Settlement Unpaid successfully." });
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
public class MarkCarrierSettlementUnPaidCommandValidator : AbstractValidator<MarkCarrierSettlementUnPaidCommand>
{
  public MarkCarrierSettlementUnPaidCommandValidator()
  {
    RuleFor(x => x.CarrierPaymentSettlementId).NotEmpty().NotEmpty().Must(GuidHelper.Validator).WithMessage(GuidHelper.GuidMessage); ;
  }
}
