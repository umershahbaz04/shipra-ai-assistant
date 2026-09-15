using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.AccountAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.AccountFeature.Commands.DeleteCarrierPaymentSettlement;
public class DeleteCarrierPaymentSettlementCommand : IRequest<ServiceResultDTO>
{
  public string? CarrierPaymentSettlementId { get; set; } 
}
public class DeleteCarrierPaymentSettlementCommandHandler : RequestHandlerBase<DeleteCarrierPaymentSettlementCommand, ServiceResultDTO>
{
  private readonly IOrderTrackingHistoryRepository _orderTrackingHistoryRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly IAccountRepository _accountRepository;

  public DeleteCarrierPaymentSettlementCommandHandler(IOrderTrackingHistoryRepository orderTrackingHistoryRepository, IOrderRepository orderRepository, IAccountRepository accountRepository, IServiceProvider serviceProvider, ILogger<DeleteCarrierPaymentSettlementCommandHandler> logger) : base(serviceProvider, logger)
  {
    _orderTrackingHistoryRepository = orderTrackingHistoryRepository;
    _orderRepository = orderRepository;
    _accountRepository = accountRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(DeleteCarrierPaymentSettlementCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var carrierPaymentSettlementId = new CarrierPaymentSettlementId(new Guid(request.CarrierPaymentSettlementId!));
      var oCarrierPaymentSettlement = await _accountRepository.GetCarrierPaymentSettlementById(carrierPaymentSettlementId);

      if (oCarrierPaymentSettlement != null)
      { 
        bool oResult = await _accountRepository.DeleteCarrierPaymentSettlement(oCarrierPaymentSettlement);
        if (oResult)
        {
          #region update order status to paid and lock order
          var oOrders = await _orderRepository.GetAllOrdersByPaymentSettlementId(carrierPaymentSettlementId);
          foreach (var oOrder in oOrders)
          {
            //remove settlement 
            oOrder.DeleteCarrierPaymentSettlement(oOrder.PaymentMethodId.GetValueOrDefault(),_currentUser.EmployeeId);
            await _orderRepository.UpdateOrder(oOrder);

            //create order note
            var oOrderTrackingHistory = await _orderTrackingHistoryRepository.CreateOrderNote(OrderNote.CreateOrderNote(oOrder!.OrderId!, "Carrier settlement deleted against " + oCarrierPaymentSettlement.PaymentRef, _currentUser.EmployeeId!));
          }
          #endregion
        }
        serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = request.CarrierPaymentSettlementId, Message = "Settlement Deleted successfully." });
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
public class DeleteCarrierPaymentSettlementCommandValidator : AbstractValidator<DeleteCarrierPaymentSettlementCommand>
{
  public DeleteCarrierPaymentSettlementCommandValidator()
  {
    RuleFor(x => x.CarrierPaymentSettlementId).NotNull().NotEmpty().Must(GuidHelper.Validator).WithMessage(GuidHelper.GuidMessage);
  }
}
