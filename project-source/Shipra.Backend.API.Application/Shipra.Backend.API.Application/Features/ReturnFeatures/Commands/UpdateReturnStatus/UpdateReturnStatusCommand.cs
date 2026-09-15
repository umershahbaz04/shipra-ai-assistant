using FluentValidation;
using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrder;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.DeleteOrder;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.ReturnAggregate;

namespace Shipra.Backend.API.Application.Features.ReturnFeatures.Commands.UpdateReturnStatus;
public class UpdateReturnStatusCommand : IRequest<ServiceResultDTO>
{
  public string? ReturnId { get; set; }
  public string? Comment { get; set; }
  public int ReturnStatusId { get; set; }
}

public class UpdateReturnStatusCommandHandler : RequestHandlerBase<UpdateReturnStatusCommand, ServiceResultDTO>
{
  private readonly IMediator _mediator;
  private readonly IOrderRepository _orderRepository;
  private readonly IEmployeeRepository _employeeRepository;
  private readonly IReturnRepository _returnRepository;

  public UpdateReturnStatusCommandHandler(IMediator mediator, IOrderRepository orderRepository, IEmployeeRepository employeeRepository, IReturnRepository returnRepository, IServiceProvider serviceProvider, ILogger<UpdateReturnStatusCommandHandler> logger) : base(serviceProvider, logger)
  {
    _mediator = mediator;
    _orderRepository = orderRepository;
    _employeeRepository = employeeRepository;
    _returnRepository = returnRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdateReturnStatusCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var objReturn = await _returnRepository.GetReturnByReturnId(new ReturnId(new Guid(request.ReturnId!)));
      if (objReturn is null)
      {
        throw new EntityNotFoundException("ClientReturnReson ", request.ReturnId!);
      }
      string? createdByName = await _employeeRepository.GetEmployeeNameById(_currentUser.EmployeeId);
      string? statusName = Enum.GetName(typeof(EnumReturnStatus), request.ReturnStatusId);
      bool isactivityLogCreated = false;
      string? activityLog = @$"Order has been {statusName} for {{orderNo}} by {createdByName}.";
      #region delete order
      if (objReturn.ReturnStatusId == (int)EnumReturnStatus.Approved)
      {
        //#region delete return order 
        Order? oldOrder = await _orderRepository.GetOrderById(objReturn.OrderId!, _currentUser.ClientId!);
        if (oldOrder is not null)
        {
          var returnOrder = await _orderRepository.GetOrderByOrderNo(oldOrder.RefNo!, _currentUser.ClientId!);
          if (returnOrder is not null)
          {
            DeleteOrdersCommand deleteOrderCommand = new DeleteOrdersCommand();
            deleteOrderCommand.OrderIds = returnOrder.OrderId!.Value.ToString(); 
            await _mediator.Send(deleteOrderCommand);

            //delete return order  
            var msg = @$"Order has been {statusName} for {returnOrder.OrderNo} by {createdByName}.";

            ReturnActivityLog returnActivityLog = ReturnActivityLog.Create(objReturn.ReturnId!, msg);
            isactivityLogCreated = await _returnRepository.CreateActivity(returnActivityLog, _currentUser.ClientIdStr!);
          }
        }
        //#endregion
      }

      #endregion

      if (request.ReturnStatusId != objReturn.ReturnStatusId)
      {
        Order? order = await _orderRepository.GetOrderById(objReturn.OrderId!, _currentUser.ClientId!);
        if (order is not null)
        {
          if (!isactivityLogCreated)
          {
            activityLog = activityLog.Replace("{orderNo}", order.OrderNo);
            ReturnActivityLog returnActivityLog = ReturnActivityLog.Create(objReturn.ReturnId!, activityLog);
            var isCreatedLog = await _returnRepository.CreateActivity(returnActivityLog, _currentUser.ClientIdStr!);
          }
          if (request.ReturnStatusId == (int)EnumReturnStatus.Approved)
          {
            var orderItems = await _orderRepository.GetOrderItemsByOrderId(objReturn.OrderId!);

            var orderAddress = await _orderRepository.GetOrderAddressById(order.OrderAddressId.GetValueOrDefault());

            CreateOrderCommand createOrderCommand = new CreateOrderCommand()
            {
              orderList = new List<CreateOrderRequestModel>()
                {
                  new CreateOrderRequestModel()
                  {
                      StoreId = order!.StoreId,
                      OrderTypeId = order.OrderTypeId,  // Default value as per your class
                      OrderDate = DateTime.UtcNow,
                      Description = order.Description,
                      Remarks = order.Remarks,
                      Amount = 0,// ammount will be 0.
                      CShippingCharges = 0,  // Assuming 'order' contains shipping charges
                      PaymentStatusId = order.PaymentStatusId,
                      Weight = order.Weight,
                      ItemValue = order.ItemValue,
                      OrderRequestVia = (int)EnumOrderRequestVia.Api,  // Default value as per your class
                      PaymentMethodId = order.PaymentMethodId,
                      StationId = order.StationId.GetValueOrDefault(),
                      Discount = 0,
                      VAT = order.Vat,
                      RefNo = order.OrderNo, //We will identify the relationship between this order and the previous order.

                      OrderNote = new OrderNoteModel() { Note = "Return Order created"},
                      OrderDeliveryTypeId = (int)EnumOrderDeliveryType.Reverse,
                      OrderAddress = new OrderAddressModel()
                      {
                       OrderAddressId = orderAddress.OrderAddressId,
                       CustomerName = orderAddress.CustomerName,
                       Email = orderAddress.Email,
                       Mobile1 = orderAddress.Mobile1,
                       Mobile2 = orderAddress.Mobile2,
                       CountryId = orderAddress.CountryId,
                       CityId = orderAddress.CityId,
                       AreaId = orderAddress.AreaId,
                       StreetAddress = orderAddress.StreetAddress,
                       StreetAddress2 = orderAddress.StreetAddress2,
                       HouseNo = orderAddress.HouseNo,
                       BuildingName = orderAddress.BuildingName,
                       Landmark = orderAddress.Landmark,
                       ProvinceId = orderAddress.ProvinceId,
                       PinCodeId = orderAddress.PinCodeId,
                       StateId = orderAddress.StateId,
                       Latitude = orderAddress.Latitude,
                       Longitude = orderAddress.Longitude,
                      },
                      OrderItems =orderItems?.Any() == true
                                  ? orderItems.Select(x => new OrderItemModel
                                  {
                                      ProductId = x.ProductId != null ? x.ProductId.Value.ToString() : "",
                                      ProductVariantId = x.ProductVariantId,
                                      Price = x.Price,
                                      Description = x.Description,
                                      Remarks = x.Remarks,
                                      Quantity = x.Quantity,
                                      Discount = x.Discount
                                  }).ToList()
                                  : null
                  }
                },
            };

            serviceResult = await _mediator.Send(createOrderCommand);
            //in case of any error while placing order
            if (!serviceResult.IsSuccess)
            {
              return serviceResult;
            }
            else
            {
              List<CreateOrderResponseDetailModel>? result = serviceResult.Result!.Data != null ? serviceResult.Result!.Data as List<CreateOrderResponseDetailModel> : null;
              if (result is not null)
              {
                var s = result.FirstOrDefault();
                if (s != null)
                {
                  objReturn.UpdateReturnStatusWithRtoorderId(request.ReturnStatusId, new OrderId(new Guid(s.OrderId!)));
                }
              }
            }

          }
          else
          {
            objReturn.UpdateReturnStatus(request.ReturnStatusId);
          }
        }
        bool? isCreated = await CreateReturnTrackingHistory(objReturn, request);
        var isAdded = await _returnRepository.UpdateReturn(objReturn);
      }
      else
      {
        serviceResult.CreateError("AlreadySameStatus", new string[] { "The status has already been updated." });
      }
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }

  private async Task<bool?> CreateReturnTrackingHistory(Return objReturn, UpdateReturnStatusCommand request)
  {
    string? createdByName = await _employeeRepository.GetEmployeeNameById(_currentUser.EmployeeId);
    var isAdded = await _returnRepository.CreateReturnTrackingHistory(ReturnTrackingHistory.Create(objReturn.ReturnId!, request.ReturnStatusId, createdByName!, request.Comment), _currentUser.ClientIdStr);
    return isAdded;
  }
}

public class UpdateReturnStatusCommandValidator : AbstractValidator<UpdateReturnStatusCommand>
{
  public UpdateReturnStatusCommandValidator()
  {
    RuleFor(x => x.ReturnStatusId).NotEmpty().NotNull().GreaterThan(0);
    RuleFor(x => x.ReturnId).NotEmpty().NotNull().Must(GuidHelper.Validator).WithMessage(GuidHelper.GuidMessage);
  }
}
