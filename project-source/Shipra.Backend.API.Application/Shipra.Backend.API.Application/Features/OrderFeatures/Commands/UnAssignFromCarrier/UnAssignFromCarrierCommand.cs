using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Result;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.MediatorNotification;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.UnAssignFromCarrier;
public class UnAssignFromCarrierCommand : IRequest<ServiceResultDTO>
{
  public int? ActiveCarrierId { get; set; }
  public string? OrderNos { get; set; }
}
public class UnAssignFromCarrierCommandHandler : RequestHandlerBase<UnAssignFromCarrierCommand, ServiceResultDTO>
{
  private readonly IMediator _mediator;
  private readonly IEmployeeRepository _employeeRepository;
  private readonly IClientRepository _clientRepository;
  private readonly IOrderRepository _orderRepository;

  public UnAssignFromCarrierCommandHandler(IMediator mediator, IEmployeeRepository employeeRepository,IClientRepository clientRepository, IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<UnAssignFromCarrierCommandHandler> logger) : base(serviceProvider, logger)
  {
    _mediator = mediator;
    _employeeRepository = employeeRepository;
    _clientRepository = clientRepository;
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UnAssignFromCarrierCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var oClinet = await _clientRepository.GetClientById(_currentUser.ClientId!);
      var orderList = await _orderRepository.GetOrdersWithOrderNos(request.OrderNos, _currentUser.ClientId!);
      #region unassign 
      //get orders which are not in house orders
      var otherThirdPartyOrders = orderList.Where(x => x.CarrierId != oClinet!.DefaultCarrierId).ToList();

      //if order tracking lock and payment settled then we cannot unassign carrier
      var discardList = otherThirdPartyOrders.Where(x => x.TrackingLock.GetValueOrDefault() || x.CarrierPaymentSettlementId != null).ToList();
      var successList = otherThirdPartyOrders.Where(x => !x.TrackingLock.GetValueOrDefault() && x.CarrierPaymentSettlementId == null && x.CarrierId != null).ToList();

      var inhouseCarrier = orderList.Where(x => x.CarrierId == oClinet!.DefaultCarrierId).ToList();
      if (inhouseCarrier.Count > 0)
      {
        serviceResult.CreateError("OrdersInHouse", new string[] { string.Join(',', $"Some Orders are my carrier orders these orders can be revert from delivery task" + string.Join(',', inhouseCarrier.Select(x => x.OrderNo))) });
        return serviceResult;
      }
      if (discardList.Count > 0)
      {
        //serviceResult.IsSuccess = false;
        //serviceResult.StatusCode = (int)HttpStatusCode.Conflict;
        //serviceResult.Errors!.Add("OrdersSettled", new string[] { string.Join(',', $"Some Orders are already settled  <br>" + discardList.Select(x => x.OrderNo)) });

        //return serviceResult;
      }
      var lockOrders = new List<Order>();
      #region get status name
      string? carrierTrackingStatusName = string.Empty;
      var carrierTrackingStatuses = await _orderRepository.GetAllCarrierTrackingStatusesByClientId(_currentUser.ClientId!);
      var objClientTrackingStatus = carrierTrackingStatuses.FirstOrDefault(x => x.CarrierTrackingStatusId == (int)EnumCarrierTrackingStatus.UnAssignfromcarrier);
      if (objClientTrackingStatus != null)
      {
        carrierTrackingStatusName = objClientTrackingStatus.TrackingStatus;
      }
      #endregion
      foreach (var oOrder in successList)
      {
        //var order = await _orderRepository.GetOrderById(orderId!); 
        if (!oOrder.TrackingLock.GetValueOrDefault(false))
        {
          oOrder!.UnAssignFromCarrier((int)EnumCarrierTrackingStatus.UnAssignfromcarrier,carrierTrackingStatusName, _currentUser.EmployeeId);
          var updatedOrder = await _orderRepository.UpdateOrder(oOrder);

          #region order history
          if (!oOrder.TrackingLock.GetValueOrDefault(false))
          {
            string? createdByName = await _employeeRepository.GetEmployeeNameById(_currentUser.EmployeeId);

            var orderHistory = OrderTrackingHistory.CreateOrderTrackingHistory(oOrder.OrderId!, (int)EnumCarrierTrackingStatus.UnAssignfromcarrier, "UnAssign from Carrier", _currentUser.EmployeeId!, createdByName);
            var createdOrderTrackingHistory = await _orderRepository.CreateOrderTrackingHistory(orderHistory);
          }
          #endregion
        }
        else
        {
          lockOrders.Add(oOrder);
        }
      }
      #endregion

      #region publish notification 
      await _mediator.Publish(new
              RequestActivityLog
      {
        Request = JsonConvert.SerializeObject(request),
        Response = JsonConvert.SerializeObject(serviceResult),
        EventName = "oncreateOrder",
        ClientId = _currentUser.ClientIdStr,
        EmployeeId = _currentUser.EmployeeIdStr,
        CreateOn = DateTime.UtcNow
      });
      #endregion
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class UnAssignFromCarrierCommandValidator : AbstractValidator<UnAssignFromCarrierCommand>
{
  public UnAssignFromCarrierCommandValidator()
  {
    RuleFor(x => x.OrderNos).NotNull().NotEmpty();
  }
}
