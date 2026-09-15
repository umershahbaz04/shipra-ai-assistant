using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.MediatorNotification;
using Shipra.Backend.API.Core.DeliveryTaskAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Command.CreateDeliveryTask;
public class CreateDeliveryTaskCommandHandler : RequestHandlerBase<CreateDeliveryTaskCommand, ServiceResultDTO>
{
  private readonly IMediator _mediator;
  private readonly IEmployeeRepository _employeeRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly IDeliveryTaskRepository _deliveryTaskRepository;
  private readonly IClientRepository _clientRepository;
  private readonly IOrderTrackingHistoryRepository _historyRepository;

  public CreateDeliveryTaskCommandHandler(IMediator mediator, IEmployeeRepository employeeRepository, IOrderRepository orderRepository, IDeliveryTaskRepository deliveryTaskRepository, IClientRepository clientRepository, IOrderTrackingHistoryRepository historyRepository, IServiceProvider serviceProvider, ILogger<CreateDeliveryTaskCommandHandler> logger) : base(serviceProvider, logger)
  {
    _mediator = mediator;
    _employeeRepository = employeeRepository;
    _orderRepository = orderRepository;
    _deliveryTaskRepository = deliveryTaskRepository;
    _clientRepository = clientRepository;
    _historyRepository = historyRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(CreateDeliveryTaskCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var orderList = await _orderRepository.GetOrdersWithOrderNos(request.OrderNos!, _currentUser.ClientId!);
      List<string> successOrderList = new List<string>();
      List<string> discardOrderList = new List<string>();
      if (orderList is not null && Enumerable.Count(orderList) > 0)
      {
        //check if order type fullfilable then order must be fullfilled 
        var unFullfilledeOrdersCheck = orderList.Where(x => x.OrderTypeId == (int)EnumOrderType.FullFilable && x.FullFillmentStatusId != (int)EnumFullfillmentStatus.Fulfilled).ToList();
        if (unFullfilledeOrdersCheck.Count > 0)
        {
          serviceResult.IsSuccess = false;
          serviceResult.StatusCode = (int)HttpStatusCode.Conflict;
          serviceResult.Errors!.Add("OrdersNotFulfilled", new string[] { $"Please fulfilled the given order(s) no. before proceed " + string.Join(',', unFullfilledeOrdersCheck.Select(x => x.OrderNo).ToList()) });

          return serviceResult;
        }

        var oClient = await _clientRepository.GetClientById(_currentUser.ClientId!);
        foreach (var oOrder in orderList)
        {
          //Check If no any carrier assigned
          if (oOrder.CarrierId is null)
          {
            var oDeliveryTask = await _deliveryTaskRepository.CreateDeliveryTask(DeliveryTask.CreateDeliveryTask(_currentUser.ClientId!, oOrder.OrderId, _currentUser.EmployeeId!));
            if (oDeliveryTask is not null)
            {
              oOrder.UpdateOrderForCreateDeliveryTask(_currentUser.EmployeeId!, oClient!.DefaultCarrierId);
              await _orderRepository.UpdateOrder(oOrder!);

              //Update order tracking history as delivered
              string? createdByName = await _employeeRepository.GetEmployeeNameById(_currentUser.EmployeeId);

              var oOrderTrackingHistory = await _historyRepository.CreateOrderTrackingHistory(OrderTrackingHistory.CreateOrderTrackingHistory(oOrder!.OrderId!, (int)EnumCarrierTrackingStatus.AssignInHouse, "Order is assigned In house", _currentUser.EmployeeId!, createdByName));

              successOrderList.Add(oOrder.OrderNo!.ToString());
            }
          }
          else
          {
            discardOrderList.Add(oOrder.OrderNo!.ToString());
          }
        }
        var result = new BaseResponseDto();
        if (discardOrderList is not null && discardOrderList.Count > 0)
        {
          result = new BaseResponseDto()
          {
            Data = string.Join(", ", discardOrderList),
            Message = "Delivery task cannot be created for these Order's because It already assigned"
          };
          serviceResult = new ServiceResultDTO(result, false);
        }
        else
        {
          result = new BaseResponseDto()
          {
            Data = string.Join(", ", successOrderList),
            Message = "Delivery task successfully created for these Order's."
          };
        }
        serviceResult = new ServiceResultDTO(result);
        serviceResult.CreateSuccessResponse(System.Net.HttpStatusCode.OK);
        #region publish notification 
        await _mediator.Publish(new
                RequestActivityLog
        {
          Request = JsonConvert.SerializeObject(request),
          Response = JsonConvert.SerializeObject(serviceResult),
          EventName = "onassigncarrier",
          ClientId = _currentUser.ClientIdStr,
          EmployeeId = _currentUser.EmployeeIdStr,
          CreateOn = DateTime.UtcNow
        });
        #endregion
        return serviceResult;
      }
      else
      {
        throw new ShipraApplicationException(System.Net.HttpStatusCode.ExpectationFailed, "Orders not found");
      }
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
