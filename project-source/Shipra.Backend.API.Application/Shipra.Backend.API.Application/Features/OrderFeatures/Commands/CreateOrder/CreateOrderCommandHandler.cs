using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Hangfire;
using iText.StyledXmlParser.Jsoup.Select;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NPOI.OpenXmlFormats.Spreadsheet;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.CarrierUseCase;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Application.DTOs.OrderUseCase.HelperOrder.UDTOrder;
using Shipra.Backend.API.Application.DTOs.SaleChannelUseCase;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrderDraft;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.IntegrationToCarrier;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Application.MediatorNotification;
using Shipra.Backend.API.Application.Services.Implementation.Modified;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Helper;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.MetaFieldAggregate;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.OrderBoxAggregate;
using Shipra.Backend.API.Core.ProductAggregate;
using Shipra.Backend.API.Core.SaleChannelConfigAggregate;
using Shipra.Backend.API.Core.ShipperInvoiceAggregate;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrder;
public class CreateOrderCommandHandler : RequestHandlerBase<CreateOrderCommand, ServiceResultDTO>
{
  private readonly IShipperInvoiceRepository _shipperInvoiceRepository;
  private readonly IMediator _mediator;
  private readonly IOrderBoxRepository _orderBoxRepository;
  private readonly IEmployeeRepository _employeeRepository;
  private readonly IOrderTrackingHistoryRepository _orderTrackingHistoryRepository;
  private readonly ICountryRepository _countryRepository;
  private readonly IProductRepository _productRepository;
  private readonly IClientRepository _clientRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly ISaleChannelFactory _saleChannelFactory;
  private readonly ISaleChannelConfigRepository _saleChannelConfigRepository;
  private readonly IShopifyRepository _shopifyRepository;
  private readonly IMetaFieldRepository _metaFieldRepository;

  public CreateOrderCommandHandler(IShipperInvoiceRepository shipperInvoiceRepository,IMediator mediator, IOrderBoxRepository orderBoxRepository, IEmployeeRepository employeeRepository, IOrderTrackingHistoryRepository orderTrackingHistoryRepository, ICountryRepository countryRepository, IProductRepository productRepository, IClientRepository clientRepository, IOrderRepository orderRepository, IServiceProvider serviceProvider, ISaleChannelFactory saleChannelFactory, ISaleChannelConfigRepository saleChannelConfigRepository, IShopifyRepository shopifyRepository, IMetaFieldRepository metaFieldRepository, ILogger<CreateOrderCommandHandler> logger) : base(serviceProvider, logger)
  {
    _shipperInvoiceRepository = shipperInvoiceRepository;
    _mediator = mediator;
    _orderBoxRepository = orderBoxRepository;
    _employeeRepository = employeeRepository;
    _orderTrackingHistoryRepository = orderTrackingHistoryRepository;
    _countryRepository = countryRepository;
    _productRepository = productRepository;
    _clientRepository = clientRepository;
    _orderRepository = orderRepository;
    _saleChannelFactory = saleChannelFactory;
    _saleChannelConfigRepository = saleChannelConfigRepository;
    _shopifyRepository = shopifyRepository;
    _metaFieldRepository = metaFieldRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateOrderCommand orderRequest, CancellationToken cancellationToken)
  {
    var response = new ServiceResultDTO();
    List<CreateOrderResponseDetailModel> orderData = new List<CreateOrderResponseDetailModel>();

    try
    {
      /// <summary>
      /// 1. create order address
      /// 2. create order
      /// 3. create order history
      /// 4. create order item
      /// 5. create ordernote
      /// 6. update product stock quantity 
      /// </summary>
      #region create draft order
      if (orderRequest.OrderDraftId.GetValueOrDefault() > 0)
      {
        var fisrtOrder = orderRequest.orderList!.FirstOrDefault();
        var OrderAddress = fisrtOrder!.OrderAddress!;
        string fullAddress = await _countryRepository.GetFullAddress(OrderAddress.StreetAddress, OrderAddress.CountryId, OrderAddress.CityId, OrderAddress.AreaId, OrderAddress.ProvinceId, OrderAddress.PinCodeId, OrderAddress.StateId);
        fisrtOrder!.OrderAddress!.CustomerFullAddress = fullAddress;
        response = await _mediator.Send(
          new CreateOrderDraftCommand
          {
            OrderDraftId = 0,
            OrderInfo = JsonConvert.SerializeObject(fisrtOrder),
            OrderTypeId = orderRequest.orderList!.FirstOrDefault()!.OrderTypeId
          }
        );

        return response;
      }
      #endregion
      #region validate fulfillable order
      List<UDTFileUploadError> validatedResponse = await ValidateProductStock(orderRequest.orderList!);
      bool isSuccessed = validatedResponse.All(x => x.IsSuccessed);

      if (!isSuccessed)
      {
        var errorDictionary = new Dictionary<string, string[]>();
        foreach (var error in validatedResponse)
        {
          if (!error.IsSuccessed) // Only add errors
          {
            string key = $"{error.Row}";
            errorDictionary[key] = error.Msg.ToArray();
          }
        }
        response.Errors = errorDictionary;
        response.IsSuccess = false;
        return response;
      }
      #endregion
      #region validate sale channel against store
      List<UDTFileUploadError> validatedSChannelResponse = await ValidateSaleChannelAgainstStore(orderRequest.orderList!);
      bool isScSuccessed = validatedResponse.All(x => x.IsSuccessed);

      if (!isSuccessed)
      {
        var errorDictionary = new Dictionary<string, string[]>();
        foreach (var error in validatedResponse)
        {
          if (!error.IsSuccessed) // Only add errors
          {
            string key = $"{error.Row}";
            errorDictionary[key] = error.Msg.ToArray();
          }
        }
        response.Errors = errorDictionary;
        response.IsSuccess = false;
        return response;
      }
      #endregion

      List<Trackingnoswithref> oAssignCarrierResponse = new();
      List<Order> successOrderListForAssignToCarrier = new();
      #region MyRegion
      foreach (var request in orderRequest.orderList!)
      {
        CreateOrderResponseDetailModel resData = new CreateOrderResponseDetailModel();

        bool isCarrierExistAndAssignOrder = request.CarrierData is not null;
        if (request.OrderItems == null || request.OrderItems.Count == 0 && request.OrderTypeId == (int)EnumOrderType.Regular)
        {
          request.OrderItems ??= new List<OrderItemModel>();

          request.OrderItems.Add(new OrderItemModel
          {
            Description = "",
            Quantity = 1 , 
          });
        }

        #region check carrier data related thing
        if (isCarrierExistAndAssignOrder) // assign order to carrier
        {
          await ProcessCarrierAssignmentAsync(request, successOrderListForAssignToCarrier, oAssignCarrierResponse, resData);
          orderData.Add(resData);
        }

        // Skip if already processed in oAssignCarrierResponse
        if (oAssignCarrierResponse.Any(x => x.shipper_Ref == request.RefNo))
          continue;
        if (successOrderListForAssignToCarrier.Any(x => x.RefNo == request.RefNo))
          continue;
        #endregion

        try
        {
          #region orderno 
          var orderNo = await _orderRepository.GetClientNextOrderNo(_currentUser.ClientId!, _currentUser.EmployeeId!, _currentUser.RoleId.GetValueOrDefault(), request.SaleChannelConfigId);

          #endregion

          #region MyRegion
          if (!string.IsNullOrEmpty(request.RefNo))
          {
            resData.RefNo = request.RefNo;
          }
          else
          {
            resData.RefNo = string.Empty;
          }
          #region order box
          if (request.OrderBoxs != null && request.OrderBoxs!.Count == 0)
          {
            var defaultBox = await _orderBoxRepository.GetDefaultClientOrderBoxById(_currentUser.ClientId!);
            if (defaultBox is not null)
            {
              request.OrderBoxs.Add(new OrderBoxRequestModel
              {
                ClientOrderBoxId = defaultBox.ClientOrderBoxId,
                Height = defaultBox.Height,
                Length = defaultBox.Length,
                Width = defaultBox.Width
              });
            }
          }
          #endregion
          if (request.OrderDeliveryTypeId == null)
          {
            request.OrderDeliveryTypeId = (int)EnumOrderDeliveryType.Forward;
          }
          #region set lat lng
          if (request.OrderAddress?.AreaId != null && request.OrderAddress?.AreaId > 0)
          {
             
          }
          #endregion
          #region order address
          var objOrderAddress = request.OrderAddress!;
          #region update street address 2
          string? modifyStreet = objOrderAddress.StreetAddress;
          if (!string.IsNullOrEmpty(objOrderAddress.StreetAddress2))
          {
            modifyStreet = objOrderAddress.StreetAddress + "," + objOrderAddress.StreetAddress2;
          }
          #endregion
          string fullAddress = await _countryRepository.GetFullAddress(modifyStreet, objOrderAddress.CountryId, objOrderAddress.CityId, objOrderAddress.AreaId, objOrderAddress.ProvinceId, objOrderAddress.PinCodeId, objOrderAddress.StateId,objOrderAddress.EntityAddressDataJson);

          if (request.OrderDeliveryTypeId.GetValueOrDefault((int)EnumOrderDeliveryType.Forward) == (int)EnumOrderDeliveryType.Reverse)
          {
            request.Amount = 0;
            request.Discount = 0;
            request.VAT = 0;
          }
          if (request.Amount.GetValueOrDefault(0) == 0)
          {
            request.PaymentMethodId = (int)EnumPaymentMethod.PP;
          }
          //if (!string.IsNullOrEmpty(objOrderAddress.Mobile1) && objOrderAddress.Mobile1.Contains("+"))
          //{
          //  request.OrderAddress!.Mobile1 = Utility.RemoveWhitespace(objOrderAddress.Mobile1);
          //  request.OrderAddress!.Mobile1 = objOrderAddress.Mobile1.Replace("+", "00");
          //}
          //if (!string.IsNullOrEmpty(objOrderAddress.Mobile2) && objOrderAddress.Mobile2.Contains("+"))
          //{
          //  request.OrderAddress!.Mobile2 = Utility.RemoveWhitespace(objOrderAddress.Mobile2!);
          //  request.OrderAddress!.Mobile2 = objOrderAddress.Mobile2.Replace("+", "00");
          //}

          var orderAddress = OrderAddress.CreateOrderAddress(request.OrderAddress!.CustomerName, fullAddress, request.OrderAddress!.Email, request.OrderAddress!.Mobile1, request.OrderAddress!.Mobile2, request.OrderAddress!.CountryId, request.OrderAddress!.CityId, request.OrderAddress!.AreaId, request.OrderAddress!.StreetAddress, request.OrderAddress!.Latitude, request.OrderAddress!.Longitude, objOrderAddress!.StreetAddress2, objOrderAddress!.HouseNo, objOrderAddress!.BuildingName, objOrderAddress!.Landmark, objOrderAddress!.ProvinceId, objOrderAddress!.PinCodeId, objOrderAddress.StateId, objOrderAddress.SelectedCarrierId, objOrderAddress.EntityAddressDataJson);

          OrderAddress createdOrderAddress = await _orderRepository.CreateOrderAddress(orderAddress);
          #endregion

          #region create order
          var client = await _clientRepository.GetClientById(_currentUser.ClientId!);
          if (client is null)
          {
            throw new EntityNotFoundException("Client ", _currentUser.ClientIdStr!.ToString());
          }
          if (request.StoreId == 0)
          {
            request.StoreId = client!.DefaultStoreId;
          }
          //if (request.StationId == 0) // order can be create without station
          //{
          //  request.StationId = client!.DefaultProductStationId.GetValueOrDefault();
          //}
          if (request.PaymentMethodId == (int)EnumPaymentMethod.PP)
          {
            request.PaymentStatusId = (int)EnumPaymentStatus.Paid;
          }
          else
          {
            request.PaymentStatusId = (int)EnumPaymentStatus.Unpaid;
          }
          #region get status name
          string? carrierTrackingStatusName = string.Empty;
          var carrierTrackingStatuses = await _orderRepository.GetAllCarrierTrackingStatusesByClientId(_currentUser.ClientId!);
          var objClientTrackingStatus = carrierTrackingStatuses.FirstOrDefault(x => x.CarrierTrackingStatusId == (int)EnumCarrierTrackingStatus.OrderPlaced);
          if (objClientTrackingStatus != null)
          {
            carrierTrackingStatusName = objClientTrackingStatus.TrackingStatus;
          }
          #endregion
          decimal? totalTax = request.OrderTaxes!.Sum(x => x.TaxValue);
          request.ItemValue = request.ItemValue != null ? request.ItemValue : request.Amount;
          var order = Order.CreateOrder(_currentUser.ClientId!, request.StoreId, request.SaleChannelConfigId, request.OrderTypeId, orderNo, request.OrderDate, orderAddress.OrderAddressId, request.Amount, request.Description, request.Remarks, request!.OrderItems?.Count(), request.CShippingCharges, request.PaymentStatusId, request.Weight, request.ItemValue, request.OrderRequestVia, request.Discount, totalTax.GetValueOrDefault(), request.PaymentMethodId, request.StationId, (int)EnumCarrierTrackingStatus.OrderPlaced, request.RefNo, _currentUser.EmployeeId!, carrierTrackingStatusName, request.SaleChannelLookupId, request.OrderDeliveryTypeId);

          //when we create FullFilable Order 
          if (request.OrderTypeId == (int)EnumOrderType.FullFilable)
          {
            order.UpdateOrderFulfillmentStatus((int)EnumFullfillmentStatus.Unfulfilled);
          }
          if (isCarrierExistAndAssignOrder)
          {
            successOrderListForAssignToCarrier.Add(order);
          }
          var createdOrder = await _orderRepository.CreateOrder(order);
          // never change response 
          resData.OrderId = createdOrder.OrderId!.Value.ToString();
          resData.OrderNo = createdOrder.OrderNo;
          resData.IsSuccess = true;
          orderData.Add(resData);

          #region CreateUpdate MetaField 
          if (request.settingConfig != null && request.settingConfig.Any())
          {
            var filteredConfig = request.settingConfig.Where(c => c.value != null && !string.IsNullOrWhiteSpace(c.value.ToString())).ToList();
            if (filteredConfig.Any())
            {
              var settingConfigJson = JsonConvert.SerializeObject(filteredConfig);
              var metafield = MetaField.CreateMetaField(settingConfigJson, createdOrder.OrderId!.Value.ToString(), _currentUser.ClientId!);
              MetaField createdMetaField = await _metaFieldRepository.CreateMetaFields(metafield);
            }
          }
          #endregion

          #region order tax
          if (request.OrderTaxes != null)
          {
            foreach (var item in request.OrderTaxes)
            {
              OrderTax orderTax = OrderTax.Create(item.ClientTaxId, item.TaxValue, createdOrder.OrderId);
              bool added = await _orderRepository.CreateOrderTax(orderTax);
            }
          }
          #endregion
          #endregion

          #region order history
          string? createdByName = await _employeeRepository.GetEmployeeNameById(_currentUser.EmployeeId);

          OrderTrackingHistory orderHistory = OrderTrackingHistory.CreateOrderTrackingHistory(createdOrder.OrderId!, (int)EnumOrderOrderTrackingHistory.ORDERPLACED, request.OrderNote?.Note, _currentUser.EmployeeId!, createdByName);
          OrderTrackingHistory createdOrderTrackingHistory = await _orderTrackingHistoryRepository.CreateOrderTrackingHistory(orderHistory);

          #endregion

          #region orderitems
          if (request.OrderItems != null)
          {
            foreach (var item in request.OrderItems)
            {
              await CreateOrderItemCommon(item, createdOrder, request.OrderTypeId);
            }
          }
          #endregion
          #region order boxes
          if (request.OrderBoxs != null)
          {
            foreach (var item in request.OrderBoxs)
            {
              // create order boxes
              OrderBox orderBox = OrderBox.Create(item.Length,item.Width,item.Height,createdOrder.OrderId);
              bool isSave = await _orderBoxRepository.CreateOrderBox(orderBox);
            }
          }
          #endregion

          #region order note
          if (!string.IsNullOrEmpty(request?.OrderNote?.Note))
          {
            var createdOrderNote = await _orderTrackingHistoryRepository.CreateOrderNote(OrderNote.CreateOrderNote(order.OrderId!, request?.OrderNote?.Note, _currentUser.EmployeeId!));
          }
          #endregion
          #endregion
        }
        catch (Exception)
        {
          resData.OrderNo = "";
          resData.OrderId = "";
          resData.IsSuccess = false;
          //response = new ServiceResultDTO(new BaseResponseDto { Data = orderData, Message = "Some orders failed to be placed successfully." });
          continue;
        }
      }

      #region assign order to third party
      var responseAssignCarrier = await AssignOrdersToCarrierAsync(orderRequest, successOrderListForAssignToCarrier);
      if (responseAssignCarrier != null && responseAssignCarrier.Result is not null)
      {
        var desAssignCarrier = JsonConvert.DeserializeObject<List<Trackingnoswithref>>(responseAssignCarrier.Result!.Data);
        if (desAssignCarrier != null)
        {
          oAssignCarrierResponse.AddRange(desAssignCarrier);
        }
      }
      #endregion
      if (orderRequest.WithThirdPartyResponse.GetValueOrDefault() || orderRequest.IsAssigCarrier.GetValueOrDefault())
      {
        dynamic finalData = new ExpandoObject();
        finalData.order = orderData; // yourList is List<Order>, List<Trackingnoswithref>, etc.
        finalData.carrierData = oAssignCarrierResponse;
        response = new ServiceResultDTO(new BaseResponseDto { Data = finalData, Message = NotificationConstants.Success });
      }
      else
      {
        response = new ServiceResultDTO(new BaseResponseDto { Data = orderData, Message = NotificationConstants.Success });
      }


      if (successOrderListForAssignToCarrier.Count > 0 && !responseAssignCarrier!.IsSuccess && responseAssignCarrier!.Errors!.Count > 0)
      {
        response.Errors = responseAssignCarrier!.Errors;
      }
      if (orderRequest.IsSaleChannelOrder.GetValueOrDefault())
      {
        var createdOrderids = string.Join(",", orderData.Select(x => x.OrderId).ToList());
        var orders = await _orderRepository.GetOrdersByOrderIds(createdOrderids, _currentUser.ClientId!);

        //group all on base of sc lookup like shopify,woocomerece
        var groupByConfigId = orders.GroupBy(x => x.SaleChannelLookupId);
        foreach (var oSaleChannelLookups in groupByConfigId)
        {
          try
          {
            int? saleChannelLookupId = oSaleChannelLookups.Key;
            // get sale channel service by lookup id
            var saleChannelService = _saleChannelFactory.GetSaleChannelService(saleChannelLookupId.GetValueOrDefault());
            CreateSaleChannelRequestModel createSaleChannelOrder = new();
            if (saleChannelService is not null)
            {
              var filterdAllSameSaleChannelOrders = orders.Where(x => x.SaleChannelLookupId == saleChannelLookupId).ToList();
              var sameSaleChannelsOrderIds = filterdAllSameSaleChannelOrders.Select(x => x.OrderId!.Value!.ToString());

              var allOrdersForCreateSaleChannelOrdersForSingleSaleChannel = orderData.Where(x => sameSaleChannelsOrderIds.Contains(x.OrderId!)).ToList();

              var singleOrder = filterdAllSameSaleChannelOrders.FirstOrDefault();
              if (singleOrder is not null)
              {
                var oShopifyConfig = await _shopifyRepository.GetShopifyConfigByClientId(singleOrder!.SaleChannelConfigId!, _currentUser.ClientId!);
                var oSaleChannelConfig = await _saleChannelConfigRepository.GetSaleChannelConfigById(singleOrder.SaleChannelConfigId.GetValueOrDefault(), _currentUser.ClientId!);

                createSaleChannelOrder.ClientId = singleOrder.ClientId;
                createSaleChannelOrder.EmployeeId = singleOrder.CreatedBy;
                createSaleChannelOrder.ShopifyConfig = oShopifyConfig;
                createSaleChannelOrder.SaleChannelConfig = oSaleChannelConfig;

                await saleChannelService.CreateSaleChannelOrderById(allOrdersForCreateSaleChannelOrdersForSingleSaleChannel, createSaleChannelOrder);
              }
            }
          }
          catch (Exception)
          {
            continue;
          }
        }
        ;
      }
      #region add order 
      var clientSetting = await _orderRepository.GetClientConfigSetting(_currentUser.ClientId!);
      bool isAllowedShipperInvocie = clientSetting?.AllowShipperInvocie ?? false;

      try
      {
        foreach (var item in orderData)
        {
          try
          {
            if (isAllowedShipperInvocie)
            {
              var order = await _orderRepository.GetOrderById(new OrderId(new Guid(item.OrderId!)), _currentUser.ClientId!);
              if (order is not null && order.SaleChannelConfigId.GetValueOrDefault() > 0)
              {
                var oAddress = await _orderRepository.GetOrderAddressById(order.OrderAddressId.GetValueOrDefault());
                var store = await _orderRepository.GetStoreWithAddress(order.StoreId.GetValueOrDefault(), _currentUser!.ClientIdStr);

                var aa = ShipperOrder.Create(order, oAddress, store);
                var a = await _shipperInvoiceRepository.CreateShipperOrder(aa);
              }
            }

          }
          catch (Exception ex)
          {
            _ = ex;
            continue;
          }
         }
        
      }
      catch (Exception ex)
      {
        _ = ex;
        
      }
      // SAME request model (no change)
      #endregion
      #region publish notification 
      await _mediator.Publish(new
              RequestActivityLog
      {
        Request = JsonConvert.SerializeObject(orderRequest),
        Response = JsonConvert.SerializeObject(response),
        EventName = ApplicationEvents.onordercreated,
        ClientId = _currentUser.ClientIdStr,
        EmployeeId = _currentUser.EmployeeIdStr,
        CreateOn = DateTime.UtcNow
      });
      #endregion

      #region check for auto create delivery task
      if (clientSetting != null && clientSetting.AutoCreateDeliveryTask.GetValueOrDefault())
      {
        var successfulOrderNos = orderData.Where(x => x.IsSuccess == true).Select(x => x.OrderNo).ToList();
        if (successfulOrderNos.Count > 0)
        {
          var createDeliveryTaskCmd = new Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Command.CreateDeliveryTask.CreateDeliveryTaskCommand 
          { 
              OrderNos = string.Join(",", successfulOrderNos) 
          };
          await _mediator.Send(createDeliveryTaskCmd, cancellationToken);
        }
      }
      #endregion

      return response;
      #endregion
    }
    catch (Exception ex)
    {
      response.CreateErrorResponse(ex);
      throw;
    }
  }
  #region process carrier code
  private async Task ProcessCarrierAssignmentAsync(
    CreateOrderRequestModel request,
    List<Order> successOrderListForAssignToCarrier,
    List<Trackingnoswithref> oAssignCarrierResponse,
    CreateOrderResponseDetailModel resData)
  {
    if (string.IsNullOrEmpty(request.RefNo))
      return;

    var oOrder = await _orderRepository.GetOrderByOrderNo(request.RefNo!, _currentUser.ClientId!);
    if (oOrder is null)
      return;
    if (oOrder.CarrierId.GetValueOrDefault() > 0)
    {
      if (oOrder.CarrierId == request.CarrierData!.CarrierId)
      {
        // send response back for assign carrier
        oAssignCarrierResponse.Add(new Trackingnoswithref
        {
          orderNo = oOrder.OrderNo,
          shipper_Ref = oOrder.RefNo,
          tracking_no = oOrder.CarrierTrackingNo
        });
      }
      else // CarrierId does not match → Unassign first
      {
        // Get carrier status name for UnAssign
        var carrierTrackingStatuses = await _orderRepository.GetAllCarrierTrackingStatusesByClientId(_currentUser.ClientId!);
        var objClientTrackingStatus = carrierTrackingStatuses
            .FirstOrDefault(x => x.CarrierTrackingStatusId == (int)EnumCarrierTrackingStatus.UnAssignfromcarrier);

        string? carrierTrackingStatusName = objClientTrackingStatus?.TrackingStatus ?? string.Empty;

        // Unassign carrier
        oOrder.UnAssignFromCarrier((int)EnumCarrierTrackingStatus.UnAssignfromcarrier, carrierTrackingStatusName, _currentUser.EmployeeId);
        var updatedOrder = await _orderRepository.UpdateOrder(oOrder);

        // Add tracking history if tracking is not locked
        if (!oOrder.TrackingLock.GetValueOrDefault(false))
        {
          string? createdByName = await _employeeRepository.GetEmployeeNameById(_currentUser.EmployeeId);

          var orderHistory = OrderTrackingHistory.CreateOrderTrackingHistory(
              oOrder.OrderId!,
              (int)EnumCarrierTrackingStatus.UnAssignfromcarrier,
              "UnAssign from Carrier",
              _currentUser.EmployeeId!,
              createdByName
          );

          var createdOrderTrackingHistory = await _orderRepository.CreateOrderTrackingHistory(orderHistory);
        }

        successOrderListForAssignToCarrier.Add(oOrder);
      }
    }
    else
    {
      successOrderListForAssignToCarrier.Add(oOrder);
    }
    resData.OrderId = oOrder.OrderId!.Value.ToString();
    resData.OrderNo = oOrder.OrderNo;
    resData.RefNo = oOrder.RefNo;
    resData.IsSuccess = true;
    resData.IsNewCreated = false;
    // send response back for assign carrier 
  }
  private async Task<ServiceResultDTO> AssignOrdersToCarrierAsync(
    CreateOrderCommand orderRequest,
    List<Order> successOrderListForAssignToCarrier)
  {
    var result = new ServiceResultDTO { IsSuccess = true };

    if (successOrderListForAssignToCarrier == null || successOrderListForAssignToCarrier.Count == 0)
    {
      return result;
    }
    var orderList = orderRequest.orderList;
    if (orderList == null || orderList.Count == 0)
    {
      return result;
    }
    var orderInfo = orderList.FirstOrDefault();
    if (orderInfo?.CarrierData == null)
    {
      return result;
    }

    var assignCarrierCommand = new AssignToCarrierCommand
    {
      CarrierId = orderInfo.CarrierData.CarrierId,
      ActiveCarrierId = orderInfo.CarrierData.ActiveCarrierId,
      orderList = new List<AssignCarrierListRequestModel>()
    };

    foreach (var item in successOrderListForAssignToCarrier)
    {
      var sOrder = orderRequest.orderList!.FirstOrDefault(x => x.RefNo == item.RefNo);
      if (sOrder?.CarrierData != null)
      {
        var objAssignOrder = new AssignCarrierListRequestModel
        {
          OrderNo = item.OrderNo,
          ActiveCarrierPickupLocationId = sOrder.CarrierData.ActiveCarrierPickupLocationId,
          CheckPikupLocation = sOrder.CarrierData.CheckPikupLocation,
          ServiceType = sOrder.CarrierData.ServiceType,
          Others = sOrder.CarrierData.Others
        };

        assignCarrierCommand.orderList.Add(objAssignOrder);
      }
    }

    result = await _mediator.Send(assignCarrierCommand);
    return result;
  }

  #endregion
  private async Task<List<UDTFileUploadError>> ValidateSaleChannelAgainstStore(List<CreateOrderRequestModel> orderList)
  {
    var errors = new List<UDTFileUploadError>();
    int count = 0;

    var allSaleChannelConfigs = await _employeeRepository.GetAllSaleChannelConfigByEmployess(_currentUser.ClientId!);

    foreach (var request in orderList!)
    {
      if (request.SaleChannelConfigId.GetValueOrDefault() > 0)
      {
        var error = new UDTFileUploadError();
        error.Row = count;
        var fileProductWithErr = new List<string>();
        count = count + 1;

        var scAgainnstStore = allSaleChannelConfigs.FirstOrDefault(x => x.StoreId == x.StoreId && x.SaleChannelConfigId == request.SaleChannelConfigId);

        if (scAgainnstStore is null)
        {
          error.IsSuccessed = false;
          var err = $"Sale person not found against selected store.";
          error.Msg.Add(err);

          errors.Add(error);
        }
      }

    }

    return errors;
  }

  private async Task<List<UDTFileUploadError>> ValidateProductStock(List<CreateOrderRequestModel> orderList)
  {
    var errors = new List<UDTFileUploadError>();
    int count = 0;
    foreach (var request in orderList!)
    {
      if (request.OrderTypeId == (int)EnumOrderType.FullFilable)
      {
        var error = new UDTFileUploadError();
        error.Row = count;
        var fileProductWithErr = new List<string>();
        count = count + 1;
        var products = await _productRepository.GetProductStocksForSelection(_currentUser.ClientId!.Value.ToString(), request.StoreId.GetValueOrDefault(), 0);

        if (products != null)
        {
          var castedStockList = (IEnumerable<dynamic>)products!;
          if (castedStockList is not null)
          {
            foreach (var item in request.OrderItems!)
            {
              var prec = castedStockList.Where(x => x.SKU?.Trim() == item.StockSku?.Trim());
              if (prec.Count() == 0)
              {
                // product not found 
                if (!string.IsNullOrEmpty(item.StockSku!))
                {
                  fileProductWithErr.Add(item.StockSku!.Trim());
                }
              }

            }
            if (fileProductWithErr.Count > 0)
            {
              var errStr = string.Join(',', fileProductWithErr.Select(x => x));
              error.IsSuccessed = false;
              var err = $"Products not found against:{errStr}";
              error.Msg.Add(err);

              errors.Add(error);
            }
          }
        }
      }

    }

    return errors;
  }
  #region MyRegion
  //var allCountries = await _countryRepository.GetAllCountries();
  //response = ValidateAddress(allCountries, orderRequest.orderList!.Select(x => x.OrderAddress).ToList());
  //private ServiceResultDTO ValidateAddress(List<Country> allCountries, List<OrderAddressModel?> list)
  //{
  //  ServiceResultDTO serviceResultDTO = new ServiceResultDTO();
  //  foreach (var item in list!)
  //  {
  //    var country = allCountries.FirstOrDefault(x => x.CountryId == item!.CountryId);
  //    if (country != null)
  //    {
  //      //var keys = country.AddressingScheme;
  //      var form = JsonConvert.DeserializeObject<CountryMainForm>(country!.AddressingScheme!);

  //      foreach (var key in form!.Keys!)
  //      {
  //        var field = form!.Fields![key].ToObject<CountryFormField>()!;


  //      }
  //    }
  //  }

  //  return serviceResultDTO;
  //}

  #endregion
  public async Task CreateOrderItemCommon(OrderItemModel item, Order createdOrder, int? orderTypeId)
  {
    OrderItem? orderItem = null;
    if (orderTypeId == (int)EnumOrderType.FullFilable)
    {
      var product = await _productRepository.GetProductByIdAsync(new ProductId(new Guid(item.ProductId!)), _currentUser.ClientId!);
      if (product is null)
      {
        throw new EntityNotFoundException("Product ", _currentUser.ClientIdStr!.ToString());
      }
      if (product!.TrackInventory.GetValueOrDefault())
      {
        //with product inventory
        #region update inventorybalance
        InventoryBalance? inventoryBalance = null;
        ProductVariant? productVariant = null;
        long? inventoryBalanceId = 0;
        long? productVariantId = item.ProductVariantId;

        if (inventoryBalanceId.GetValueOrDefault() == 0 && productVariantId.GetValueOrDefault() > 0 && createdOrder.StationId.HasValue && createdOrder.StationId.Value > 0)
        {
          var resolvedBalance = await _productRepository.GetInventoryBalanceByVariantAndStationAsync(productVariantId.GetValueOrDefault(), createdOrder.StationId.Value);
          if (resolvedBalance != null)
          {
            inventoryBalanceId = resolvedBalance.InventoryBalanceId;
          }
        }

        if (inventoryBalanceId.GetValueOrDefault() > 0)
        {
          inventoryBalance = await _productRepository.GetInventoryBalanceByIdAsync(inventoryBalanceId.GetValueOrDefault());
          if (inventoryBalance is null)
          {
            throw new EntityNotFoundException("InventoryBalance ", inventoryBalanceId.GetValueOrDefault());
          }
          var quantityCommited = inventoryBalance.QuantityCommitted + item.Quantity.GetValueOrDefault();
          var quantityAvailable = inventoryBalance.QuantityAvailable - item.Quantity.GetValueOrDefault();
          inventoryBalance.UpdateQuantities(inventoryBalance.QuantityOnHand, quantityAvailable, quantityCommited);
          await _productRepository.UpdateInventoryBalanceAsync(inventoryBalance);

          productVariantId = inventoryBalance.ProductVariantId;
        }

        if (productVariantId.GetValueOrDefault() > 0)
        {
          productVariant = await _productRepository.GetProductVariantByIdAsync(productVariantId.GetValueOrDefault());
        }
        #endregion
        if (productVariant != null)
        {
          item.Description = OrderCommon.GetDescriptionForShipmentItem(product, productVariant);
        }
        else
        {
          item.Description = "";
        }
        orderItem = OrderItem.CreateOrderItemFullFilable(createdOrder.OrderId!, new ProductId(new Guid(item.ProductId!)), item.Price, item.Description, item.Remarks, item.Quantity, item.Discount, item.HsCode, item.OriginCountryCode, item.UnitRate, item.Weight, productVariantId);
        OrderItem createdItem = await _orderRepository.CreateOrderItem(orderItem!);
      }
      else
      {
        orderItem = OrderItem.CreateOrderItemFullFilable(createdOrder.OrderId!, new ProductId(new Guid(item.ProductId!)), item.Price, item.Description, item.Remarks, item.Quantity, item.Discount, item.HsCode, item.OriginCountryCode, item.UnitRate, item.Weight, item.ProductVariantId);
        OrderItem createdItem = await _orderRepository.CreateOrderItem(orderItem!);
      }
    }
    else if (orderTypeId == (int)EnumOrderType.Regular)
    {
      orderItem = OrderItem.CreateOrderItemRegular(createdOrder.OrderId!, item.Price, item.Description, item.Remarks, item.Quantity, item.Discount,item.HsCode,item.OriginCountryCode,item.UnitRate,item.Weight);
      OrderItem createdItem = await _orderRepository.CreateOrderItem(orderItem!);
    }
  }
}
