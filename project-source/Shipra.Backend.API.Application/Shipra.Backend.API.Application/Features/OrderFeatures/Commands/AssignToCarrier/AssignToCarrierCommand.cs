using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Nancy;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.CarrierUseCase;
using Shipra.Backend.API.Application.MediatorNotification;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.WalletAggregate;
using Shipra.Backend.API.SharedKernel.Interfaces;
using Shipra.Backend.API.SharedKernel.Models;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.IntegrationToCarrier;
public class AssignToCarrierCommand : AssignToCarrierRequestModel,IRequest<ServiceResultDTO>
{ 
}

public class IntegrationToCarrierCommandHandler : RequestHandlerBase<AssignToCarrierCommand, ServiceResultDTO>
{
  private readonly IClientRepository _clientRepository;
  private readonly IWalletRepository _walletRepository;
  private readonly IMediator _mediator;
  private readonly IEmployeeRepository _employeeRepository;
  private readonly IOrderTrackingHistoryRepository _orderTrackingHistoryRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly IConfigRepository _configRepository;
  private readonly ICarrierSharedRepository _carrierSharedRepository;
  private readonly ICarrierRepository _carrierRepository;

  public IntegrationToCarrierCommandHandler(IClientRepository clientRepository, IWalletRepository walletRepository, IMediator mediator, IEmployeeRepository employeeRepository, IOrderTrackingHistoryRepository orderTrackingHistoryRepository, IOrderRepository orderRepository, IConfigRepository configRepository, ICarrierSharedRepository carrierSharedRepository, ICarrierRepository carrierRepository, IServiceProvider serviceProvider, ILogger<IntegrationToCarrierCommandHandler> logger) : base(serviceProvider, logger)
  {
    _clientRepository = clientRepository;
    _walletRepository = walletRepository;
    _mediator = mediator;
    _employeeRepository = employeeRepository;
    _orderTrackingHistoryRepository = orderTrackingHistoryRepository;
    _orderRepository = orderRepository;
    _configRepository = configRepository;
    _carrierSharedRepository = carrierSharedRepository;
    _carrierRepository = carrierRepository;
  }


  protected override async Task<ServiceResultDTO> HandleRequest(AssignToCarrierCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var client = await _clientRepository.GetClientById(_currentUser.ClientId!);
      if (client is null)
      {
        throw new EntityNotFoundException("Client ", _currentUser.ClientIdStr!.ToString());
      }

      ActiveCarrierContractResponseModel? activeCarrier = null;
      Wallet? oWallet = null;
      if (request.CarrierContractTypeId == (int)EnumCarrierContractType.ShipraContractType)
      {
        oWallet = await _walletRepository.GetWalletByClientId(_currentUser.ClientId!);
        if (!client.AllowWithoutBalance.GetValueOrDefault())
        {
          if (oWallet is null)
          {
            throw new EntityNotFoundException("Wallet", _currentUser.ClientId!);
          }

          if (oWallet.AvailableBalance == 0 && oWallet.CurrentBalance == 0)
          {
            serviceResult.CreateError("LowBalance", new string[] { "Your wallet balance is low. Please recharge it." });
            return serviceResult;
          }
        }

        ShipraContractCarrier? shipraContractCarrier = await _carrierRepository.GetShipraContractCarrierByCarrierId(request.CarrierId);

        if (shipraContractCarrier == null)
        {
          throw new EntityNotFoundException("ShipraContractCarrier", request.CarrierId);
        }


        ShipraContractClientCarrier? shipraContractClientCarrier = await _carrierRepository.GetShipraContractClientCarrier(_currentUser.ClientId!, shipraContractCarrier.ShipraContractCarrierId);

        if (shipraContractClientCarrier == null)
        {
          throw new EntityNotFoundException("ShipraContractClientCarrier", request.CarrierId);
        }

        //check if !AllowCodOrder then return with message else continue
        if (!shipraContractClientCarrier.AllowCodOrder.GetValueOrDefault())
        {
          throw new Exception("You cannot assign the order using Shipra client carrier. Please contact with support. ");
        }
        activeCarrier = _mapper.Map<ActiveCarrierContractResponseModel>(shipraContractCarrier);

      }
      else
      {

        ActiveCarrier? oActiveCarrier = await _carrierRepository.GetActiveCarrierByActiveCarrierId(request.ActiveCarrierId, _currentUser.ClientId!);
        if (oActiveCarrier == null)
        {
          throw new EntityNotFoundException("ActiveCarrier", request.CarrierId);
        }
        activeCarrier = _mapper.Map<ActiveCarrierContractResponseModel>(oActiveCarrier);

      }
      string? orderNos = string.Join(",", request!.orderList!.Select(x => x.OrderNo));
      var orderList = await _orderRepository.GetOrdersWithOrderNos(orderNos, _currentUser.ClientId!);
      //check if order type fullfilable then order must be fullfilled 
      var unFullfilledeOrdersCheck = orderList.Where(x => x.OrderTypeId == (int)EnumOrderType.FullFilable && x.FullFillmentStatusId != (int)EnumFullfillmentStatus.Fulfilled).ToList();
      var lockedOrdersCheck = orderList.Where(x => x.TrackingLock == true).ToList();
      if (lockedOrdersCheck.Count > 0)
      {
        serviceResult.IsSuccess = false;
        serviceResult.StatusCode = (int)HttpStatusCode.Conflict;
        serviceResult.Errors!.Add("OrdersLocked", new string[] { $"you cannot assign locked order,Please remove the given order(s) no. before proceed " + string.Join(',', lockedOrdersCheck.Select(x => x.OrderNo).ToList()) });

        return serviceResult;
      }
      if (false && unFullfilledeOrdersCheck.Count > 0)
      {
        serviceResult.IsSuccess = false;
        serviceResult.StatusCode = (int)HttpStatusCode.Conflict;
        serviceResult.Errors!.Add("OrdersNotFulfilled", new string[] { $"Please fulfilled the given order(s) no. before proceed " + string.Join(',', unFullfilledeOrdersCheck.Select(x => x.OrderNo).ToList()) });

        return serviceResult;
      }

      //checking if order already assign to carrier 
      var existCarrier = orderList.Where(x => x.CarrierId > 0 && x.CarrierId != null).ToList();
      if (existCarrier.Count() == 0)
      {
        var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.IntegrationKey, _currentUser.EnvironmentTypeId);
        if (mcconfig is null)
        {
          throw new EntityNotFoundException("Mcconfig", "Integration Value");
        }

        var clientId = _currentUser.ClientIdStr!;
        List<AssignCarrierRequestModel>? data = request!.orderList!.Select(x => new AssignCarrierRequestModel { ActiveCarrierPickupLocationId = x.ActiveCarrierPickupLocationId, OrderNo = x.OrderNo , ServiceType = x.ServiceType,Others = x.Others }).ToList();
        var carreirresult = await _carrierSharedRepository.AssignToCarrier(activeCarrier.CarrierId, activeCarrier.ActiveCarrierId, clientId, orderNos,data, request.CarrierContractTypeId, mcconfig.Value!,null,request.IsDispatchEx);

        if (!string.IsNullOrEmpty(carreirresult))
        {
          IntegrationCarrierResponseModel<List<Trackingnoswithref>> result = JsonConvert.DeserializeObject<IntegrationCarrierResponseModel<List<Trackingnoswithref>>>(carreirresult);
          // we cannot check issucess value here because now we have both kind of order like success/faild so handle both inside
          if (result != null)
          {
            if (result!.data! != null)
            {
              #region get status name
              string? carrierTrackingStatusName = string.Empty;
              var carrierTrackingStatuses = await _orderRepository.GetAllCarrierTrackingStatusesByClientId(_currentUser.ClientId!);
              var objClientTrackingStatus = carrierTrackingStatuses.FirstOrDefault(x => x.CarrierTrackingStatusId == (int)EnumCarrierTrackingStatus.AssignedTocarrier);
              if (objClientTrackingStatus != null)
              {
                carrierTrackingStatusName = objClientTrackingStatus.TrackingStatus;
              }
              #endregion
              //update order and relevent data
              foreach (var item in result!.data!)
              {
                var oOrderForUpdate = orderList.FirstOrDefault(x => x.OrderNo == item.orderNo);

                if (oOrderForUpdate is not null)
                {
                  var orderId = oOrderForUpdate!.OrderId;

                  var order = await _orderRepository.GetOrderById(orderId!, _currentUser.ClientId!);
                  if (order != null)
                  {
                    if (!client.AllowWithoutBalance.GetValueOrDefault())
                    {
                      #region if contract type shipra then deduct wallet 
                      if (request.CarrierContractTypeId == (int)EnumCarrierContractType.ShipraContractType)
                      {
                        oWallet!.DeductBalanceOnAssignOrder(activeCarrier.FlatRate.GetValueOrDefault());
                        var isWalletUpdate = await _walletRepository.UpdateWallet(oWallet);
                        if (isWalletUpdate)
                        {
                          #region create transactio
                          string transNo = Transaction.GetTransactionNo(client.ClientIdentifier);
                          Transaction transaction = Transaction.CreateTransaction(transNo, $"Wallet with an amount of {activeCarrier.FlatRate.GetValueOrDefault()} has been debited against Order No: {order.OrderNo}.", _currentUser.ClientId, oWallet.WalletId, (int)EnumTransactionTypeLookup.ServicesChargeDebit, 0, activeCarrier.FlatRate.GetValueOrDefault());

                          var isCreated = await _walletRepository.CreateTransaction(transaction);
                          #endregion
                        }
                        //update deliver charges
                        order.UpdateSerViceCharges(activeCarrier.FlatRate.GetValueOrDefault());
                      }
                      #endregion
                    }
                    if (!order.TrackingLock.GetValueOrDefault(false))
                    {
                      var activeCarrierPickupLocation = request.orderList!.Select(x => x.ActiveCarrierPickupLocationId ?? 0).FirstOrDefault();
                      order!.UpdateAssignToCarrierTrackingNos(request.CarrierId, activeCarrier.ActiveCarrierId, activeCarrierPickupLocation, request.CarrierContractTypeId, item!.tracking_no!, carrierTrackingStatusName!, (int)EnumCarrierTrackingStatus.AssignedTocarrier, _currentUser.EmployeeId);
                      order.UpdateSerViceCharges(request.DeliveryCharges ?? 0);
                      var updatedOrder = await _orderRepository.UpdateOrder(order);

                      #region order history

                      string? createdByName = await _employeeRepository.GetEmployeeNameById(_currentUser.EmployeeId);

                      var historyNote = carrierTrackingStatusName;
                      var orderHistory = OrderTrackingHistory.CreateOrderTrackingHistory(order.OrderId!, (int)EnumCarrierTrackingStatus.AssignedTocarrier, historyNote, _currentUser.EmployeeId!, createdByName);
                      var createdOrderTrackingHistory = await _orderRepository.CreateOrderTrackingHistory(orderHistory);
                    }
                    #endregion
                  }
                }

              }
            }

            var orderErrors = Utils.GetValueFromDictionryByKey("orderErrors", result!.errors!);
            if (result.configErrors?.Count > 0)
            {
              serviceResult.Errors = result!.configErrors;
              serviceResult.IsSuccess = false;
              return serviceResult;
            }
            if (!string.IsNullOrEmpty(orderErrors))
            {
              #region get status name
              string? carrierTrackingStatusName = string.Empty;
              var carrierTrackingStatuses = await _orderRepository.GetAllCarrierTrackingStatusesByClientId(_currentUser.ClientId!);
              var objClientTrackingStatus = carrierTrackingStatuses.FirstOrDefault(x => x.CarrierTrackingStatusId == (int)EnumCarrierTrackingStatus.AssignedTocarrierFaild);
              if (objClientTrackingStatus != null)
              {
                carrierTrackingStatusName = objClientTrackingStatus.TrackingStatus;
              }
              #endregion

              serviceResult.Errors = new();
              var orderWithErrors = JsonConvert.DeserializeObject<List<OrderErrorResponseModel>>(orderErrors!);
              foreach (var item in orderWithErrors!)
              {
                var oOrderForUpdate = orderList.FirstOrDefault(x => x.OrderNo == item.OrderNo);

                if (oOrderForUpdate is not null)
                {
                  var orderId = oOrderForUpdate!.OrderId;

                  var order = await _orderRepository.GetOrderById(orderId!, _currentUser.ClientId!);
                  if (order != null)
                  {
                    if (!order.TrackingLock.GetValueOrDefault(false))
                    {
                      order!.UpdateAssignToCarrierFaild((int)EnumCarrierTrackingStatus.AssignedTocarrierFaild, _currentUser.EmployeeId, carrierTrackingStatusName);
                      var updatedOrder = await _orderRepository.UpdateOrder(order);

                      #region order history

                      string? createdByName = await _employeeRepository.GetEmployeeNameById(_currentUser.EmployeeId);

                      var historyNote = "Assign To Carrier Faild";
                      var orderHistory = OrderTrackingHistory.CreateOrderTrackingHistory(order.OrderId!, (int)EnumCarrierTrackingStatus.AssignedTocarrierFaild, historyNote, _currentUser.EmployeeId!, createdByName);
                      var createdOrderTrackingHistory = await _orderRepository.CreateOrderTrackingHistory(orderHistory);
                      #endregion
                      #region create note
                      //create order note
                      var oOrderTrackingHistory = await _orderTrackingHistoryRepository.CreateOrderNote(OrderNote.CreateOrderNote(orderId!, $"Order Assign to carrier Faild against {activeCarrier.CarrierAlias}", _currentUser.EmployeeId!));
                      #endregion
                    }
                  }
                }

                serviceResult.Errors.Add(item.OrderNo!, new string[] { item.Messages! });
                serviceResult.IsSuccess = false;
              }
            }
            else
            {
              serviceResult = new ServiceResultDTO(result!.data!);
              serviceResult.CreateSuccessResponse();
            }
          }
          else
          {
            serviceResult.IsSuccess = false;
            serviceResult.Errors = result!.errors;
            serviceResult.ConfigErrors = result!.configErrors;
          }
        }
        else
        {
          serviceResult.IsSuccess = false;
          serviceResult.Errors!.Add("ThirdPartyError", new string[] { "Error while placing order" });
        }

      }
      else
      {
        serviceResult.IsSuccess = false;
        serviceResult.StatusCode = (int)HttpStatusCode.Conflict;
        serviceResult.Errors!.Add("AlreadyAssigned", new string[] { "Following orders are already assigned. " + string.Join(',', existCarrier.Select(x => x.OrderNo)) });
      }
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
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class AssignToCarrierCommandValidator : AbstractValidator<AssignToCarrierCommand>
{
  public AssignToCarrierCommandValidator()
  {
    RuleFor(x => x.CarrierId).NotNull().NotEmpty().GreaterThan(0);
    When(v => v.CarrierContractTypeId > 0 && v.CarrierContractTypeId == (int)EnumCarrierContractType.OwnContractType, () =>
    {
      RuleFor(x => x.ActiveCarrierId).NotNull().NotEmpty().GreaterThan(0);
    });

    RuleFor(x => x.orderList).Must(x => x != null).WithMessage("OrderItems list must contain at least one item.");
    RuleForEach(x => x.orderList).SetValidator(x => new AssignCarrierListRequestModelValidator());

  }
}
public class AssignCarrierListRequestModelValidator : AbstractValidator<AssignCarrierListRequestModel>
{
  public AssignCarrierListRequestModelValidator()
  {
    RuleFor(v => v.OrderNo).NotNull().NotEmpty();
    When(v => v.CheckPikupLocation.GetValueOrDefault(), () =>
    {
      RuleFor(v => v.ActiveCarrierPickupLocationId).NotNull().NotEmpty().GreaterThan(0); 
    });

  }
}
