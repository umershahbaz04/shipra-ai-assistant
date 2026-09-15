using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.AccountAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.AccountFeature.Commands.CreateCarrierPaymentSettlement;

public class CreateCarrierPaymentSettlementCommandHandler : RequestHandlerBase<CreateCarrierPaymentSettlementCommand, ServiceResultDTO>
{
  private readonly IOrderRepository _orderRepository;
  private readonly IOrderTrackingHistoryRepository _orderTrackingHistoryRepository;
  private readonly IAccountRepository _accountRepository;

  public CreateCarrierPaymentSettlementCommandHandler(IOrderRepository orderRepository, IOrderTrackingHistoryRepository orderTrackingHistoryRepository, IAccountRepository accountRepository, IServiceProvider serviceProvider, ILogger<CreateCarrierPaymentSettlementCommandHandler> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;
    _orderTrackingHistoryRepository = orderTrackingHistoryRepository;
    _accountRepository = accountRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateCarrierPaymentSettlementCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {

      List<string> ids = new List<string>();
      var orders = await _orderRepository.GetOrdersWithOrderNos(string.Join(',', request.list!.Select(x => x.OrderNo)), _currentUser.ClientId!);
      var notSameCarrierList = orders.Where(x => x.CarrierId != request.CarrierId).ToList();
      if (notSameCarrierList.Count == 0)
      {
        var alreadySettledup = orders.Where(x => x.CarrierPaymentSettlementId != null).ToList();
        if (alreadySettledup.Count == 0)
        {

          var groupData = request.list!.GroupBy(x => x.PaymentRef);

          foreach (var group in groupData)
          {

            var totalAmount = group.Sum(x => x.Amount);
            var firstRecord = group.FirstOrDefault();

            CarrierPaymentSettlement carrierPaymentSettlement = CarrierPaymentSettlement.CreateCarrierPaymentSettlement(totalAmount, firstRecord?.PaymentRef, firstRecord?.PaymentDate, request.CarrierId, _currentUser.EmployeeId!);
            var ccCarrierPaymentSettlement = await _accountRepository.CreateCarrierPaymentSettlement(carrierPaymentSettlement);
            ids.Add(ccCarrierPaymentSettlement.CarrierPaymentSettlementId!.Value.ToString());

            foreach (var item in group)
            {
              var order = await _orderRepository.GetOrderByOrderNo(item.OrderNo!, _currentUser.ClientId!);

              if (order == null)
              {
                throw new EntityNotFoundException("Order ", item.OrderNo!);
              }

              #region update order
              order.UpdateCreateCarrierPaymentSettlement(ccCarrierPaymentSettlement.CarrierPaymentSettlementId, _currentUser.EmployeeId);
              var eo = await _orderRepository.UpdateOrder(order);

              //create order note
              var oOrderTrackingHistory = await _orderTrackingHistoryRepository.CreateOrderNote(OrderNote.CreateOrderNote(order!.OrderId!, "Carrier settlement created against " + carrierPaymentSettlement.PaymentRef, _currentUser.EmployeeId!));
              #endregion
            }
          }
          serviceResult = new ServiceResultDTO(new BaseResponseDto() { Data = string.Join(',', ids.Select(x => x)), Message = NotificationConstants.Success });
        }
        else
        {
          serviceResult.IsSuccess = false;
          serviceResult.Errors!.Add("AlreadySettledup", new string[] { "Some orders are already settled up please fix following   " + string.Join(',', orders!.Select(x => x.OrderNo)) });
        }
      }
      else
      {

        serviceResult.Errors!.Add("NotSameCarrierOrders", new string[] { "Some order are not belongs to same carrier  " + string.Join(',', notSameCarrierList.Select(x => x.OrderNo).ToList()) });
        serviceResult.IsSuccess = false;
        return serviceResult;
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
