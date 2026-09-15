using Hangfire;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.DTOs.CarrierUseCase;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Application.DTOs.SaleChannelUseCase;
using Shipra.Backend.API.Application.DTOs.SaleChannelUseCase.ShopifyOrder;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrder;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.ProductAggregate;
using Shipra.Backend.API.Core.SaleChannelConfigAggregate;
using Shipra.Backend.API.Core.SaleChannelOrderAggregate;
using Shipra.Backend.API.Core.ShopifyAggregate;
using Shipra.Backend.API.Core.StoresAggregate;
using Shipra.Backend.API.SharedKernel.Interfaces;
using ShopifySharp;
using ShopifySharp.Filters;
using static Shipra.Backend.API.Application.Helpers.Utility;
using Order = Shipra.Backend.API.Core.OrderAggregate.Order;

namespace Shipra.Backend.API.Application.Services.Implementation.Modified;
public class ShopifyService : ISaleChannelService
{
  private readonly IJobRepository _jobRepository;
  private readonly ICountryRepository _countryRepository;
  private readonly ISharedCreateOrderShipraRepository _sharedCreateOrderShipraRepository;
  private readonly IConfigRepository _configRepository;

  public ShopifyService(IJobRepository jobRepository, ICountryRepository countryRepository, ISharedCreateOrderShipraRepository sharedCreateOrderShipraRepository, IConfigRepository configRepository)
  {
    _jobRepository = jobRepository;
    _countryRepository = countryRepository;
    _sharedCreateOrderShipraRepository = sharedCreateOrderShipraRepository;
    _configRepository = configRepository;
  }

  public async Task ProcessSaleChannelOrdersAsync(IEnumerable<SaleChannelConfigResponseModel> filterdOrderData)
  {
    foreach (var oSaleChannelsResponse in filterdOrderData)
    {
      string orderType = Utils.GetClientSettingValueWithByKey(oSaleChannelsResponse.SCSettingConfig, "order", "orderType");
      string autoFulfill = Utils.GetClientSettingValueWithByKey(oSaleChannelsResponse.SCSettingConfig, "order", "autoFulfill");

      orderType = !string.IsNullOrEmpty(orderType) ? orderType.Trim().ToLower() : string.Empty;
      // Convert to bool using TryParse
      bool isAutoFulfill;
      bool.TryParse(autoFulfill, out isAutoFulfill);

      var oSaleChannelConfig = SaleChannelConfig.ConverSaleChannelConfig(oSaleChannelsResponse.SaleChannelConfigId, oSaleChannelsResponse.StoreId, oSaleChannelsResponse.SaleChannelLookupId, oSaleChannelsResponse.SaleChannelName!, new ClientId(oSaleChannelsResponse!.ClientId!.Value), new EmployeeId(oSaleChannelsResponse!.ClientId!.Value), oSaleChannelsResponse.SCSettingConfig, oSaleChannelsResponse.SaleChannelKey, oSaleChannelsResponse.UserName, oSaleChannelsResponse.Password);

      if (!string.IsNullOrEmpty(oSaleChannelConfig.UserName) && !string.IsNullOrEmpty(oSaleChannelConfig.Password))
      {
        var saleChannelConfigId = oSaleChannelsResponse.SaleChannelConfigId;
        var clientId = new ClientId(oSaleChannelsResponse!.ClientId!.Value);
        var employeeId = new EmployeeId(oSaleChannelsResponse!.ClientId!.Value);

        var oStore = await _jobRepository.GetStoreById(oSaleChannelConfig.StoreId.GetValueOrDefault(), clientId!);

        await ProcessOrdersAsync(saleChannelConfigId, clientId, oStore!, oSaleChannelConfig, oSaleChannelsResponse, orderType, employeeId, isAutoFulfill);
      }
    }

  }

  private async Task ProcessOrdersAsync(int? saleChannelConfigId, ClientId clientId, Store oStore, SaleChannelConfig oSaleChannelConfig, SaleChannelConfigResponseModel oSaleChannelsResponse, string orderType, EmployeeId employeeId, bool isAutoFulfill)
  {
    orderType = !string.IsNullOrEmpty(orderType) ? orderType.Trim().ToLower() : string.Empty;
    // 1. Validate to get Shopify config
    var oShopifyConfig = await _jobRepository.GetShopifyConfigByClientId(saleChannelConfigId, clientId);
    if (oShopifyConfig == null || oShopifyConfig.AccessToken == null) return;

    // Assuming you have a LocationManager instance
    var locationManager = new LocationManager(_countryRepository);

    // Retrieve all geographic data
    var (countries, cities, provinces, states, areas, pinCodes) = await locationManager.GetAllLocationDataAsync();
    if (oShopifyConfig is not null && oShopifyConfig.AccessToken is not null)
    {
      //2. Fetch Orders data from shopify
      var sdate = DateTime.Now.AddDays(-30);
      var shopifyOrderList = await GetAllOrdersListOnShopForShopify(oShopifyConfig.ShopDomain!, oShopifyConfig.AccessToken!, Utility.ConvertUTCDateToDateTimeOffSet(DateTime.UtcNow.AddDays(-30)), Utility.ConvertUTCDateToDateTimeOffSet(DateTime.UtcNow));
      if (shopifyOrderList is not null && shopifyOrderList.Count > 0)
      {
        //3. Convert new order data OrderId to String (,) Ids
        var shopifyOrderIds = string.Join(',', shopifyOrderList.Select(x => x.Id));

        //4. Get record from existing Shipra sale order data to compare with new shopify order list
        var oSaleChannelOrderList = await _jobRepository.GetSaleChannelOrderListByOrderIds(shopifyOrderIds, (int)EnumSaleChannelLookup.Shopify, clientId!);

        //dynamic data;

        if (oSaleChannelOrderList is not null && oSaleChannelOrderList.Count > 0)
        {
          //5 If order found in sale channel Order Data then first get orderIds
          var saleChannelOrderIds = oSaleChannelOrderList.Select(x => x.OrderId!).ToList();

          //6 Convert the list of strings to a list of long values using LINQ
          var longList = saleChannelOrderIds.Select(s => long.Parse(s!)).ToList();

          //7. Discard those record which already exist
          shopifyOrderList = shopifyOrderList.Where(x => !longList!.Contains((long)x.Id!)).ToList();
        }
        //Filter those oders from list have no customer address and zero Item count
        shopifyOrderList = shopifyOrderList.Where(x => x.ShippingAddress != null && x.ShippingAddress.Name != null && x.ShippingAddress.Address1 != null && x.LineItems.Count() > 0).ToList();
        if (orderType == EnumOrderType.Regular.ToString().ToLower())
        {
          //concider all order as regular
          await CreateShopifyOrder(shopifyOrderList, oStore!, countries, cities, provinces, states, areas, pinCodes, oSaleChannelConfig, (int)EnumOrderType.Regular, clientId, employeeId, isAutoFulfill);

          // Schedule a recurring job that runs every 5 minutes
          //RecurringJob.AddOrUpdate(
          //$"{oSaleChannelsResponse.SaleChannelName}_RecurringJob_{orderType}",            // A unique identifier for the recurring job
          //() => RecurringJobMethod(mcconfig.Value!),              // Method to call
          //   "*/1 * * * *"                          // Cron expression for every 5 minutes
          //);
        }
        else if (orderType == EnumOrderType.FullFilable.ToString().ToLower())
        {
          //Filter those oders from list have  SKUs
          shopifyOrderList = shopifyOrderList.Where(x => x.LineItems.Any(v => v.SKU != null && v.SKU != "")).ToList();
          await CreateShopifyOrder(shopifyOrderList, oStore!, countries, cities, provinces, states, areas, pinCodes, oSaleChannelConfig, (int)EnumOrderType.FullFilable, clientId, employeeId, isAutoFulfill);
        }
        else if (orderType == EnumOrderType.Both.ToString().ToLower())
        {
          var shopifyOrderListForRegular = shopifyOrderList.Where(x => x.LineItems.Any(v => v.SKU == null && v.VariantId == null)).ToList();
          if (shopifyOrderListForRegular.Count > 0)
          {
            await CreateShopifyOrder(shopifyOrderListForRegular, oStore!, countries, cities, provinces, states, areas, pinCodes, oSaleChannelConfig, (int)EnumOrderType.Regular, clientId, employeeId, isAutoFulfill);
          }

          //Filter those oders from list have  SKUs
          var shopifyOrderListForFulillable = shopifyOrderList.Where(x => x.LineItems.Any(v => v.SKU != null && v.SKU != "")).ToList();
          if (shopifyOrderListForFulillable.Count > 0)
          {
            await CreateShopifyOrder(shopifyOrderListForFulillable, oStore!, countries, cities, provinces, states, areas, pinCodes, oSaleChannelConfig, (int)EnumOrderType.FullFilable, clientId, employeeId, isAutoFulfill);
          }
        }
      }
      else
      {
        // log something
      }
    }
    else
    {
      // log something
    }
  }

  #region create order
  #region regular orer  
  public async Task CreateShopifyOrder(
    List<ShopifySharp.Order> shopifyOrderList,
    Store oStore,
    List<Core.CountryAggregate.Country> countries,
    List<City> cities,
    List<Core.CountryAggregate.Province> provinces,
    List<State> states,
    List<Area> areas,
    List<PinCode> pinCodes,
    SaleChannelConfig oSaleChannelConfig,
    int orderTypeId,
    ClientId clientId,
    EmployeeId employeeId, bool isAutoFulfill)
  {
    //Filter and select shopify orders line items SKUs to match with Shipra
    var shopifyProductSKUsList = shopifyOrderList!.SelectMany(order => order.LineItems).Select(lineItem => lineItem.SKU);
    //Join list to comman seprated string
    var shopifyProductSKUs = string.Join(",", shopifyProductSKUsList);
    //Get all match SKUs product from shipra product stock
    var oShipraProductList = await _jobRepository.GetAllProductStocksForSaleChannelInventorySync(clientId!.Value.ToString()!, shopifyProductSKUs);
    var castedList = (IEnumerable<dynamic>)oShipraProductList!;
    //Filter Shipra Product SKUs List
    var productSKUs = castedList!.Select(p => p.SKU).ToList();
    //Filter record from shipra order list get only dose oders match with Shipra stock
    var matchedOrders = shopifyOrderList.Where(order => order.LineItems.Any(item => productSKUs.Contains(item.SKU))).ToList();

    ShopifyOrderDetailResponse response = ShopifyOrderDetailSimplified.ConverShopifytoShipraOrderDetail(matchedOrders, countries, cities, provinces, states, areas, pinCodes, oStore, (int)EnumStationLookup.Dubai, oSaleChannelConfig, (int)EnumSaleChannelLookup.Shopify, orderTypeId, castedList);

    var filtedredOrderWithoutErrors = FilterOrdersWithNoErrors(response);
    //    oSaleChannelOrder = SaleChannelOrder.CreateSaleChannelOrder(oSaleChannelConfig.SaleChannelLookupId, oSaleChannelConfig.SaleChannelConfigId, request.SCOrderId!, request.SCOrderNo, shopifyJson, createdDAte, createdOrder.OrderId!, clientId, employeeId);

    var list = filtedredOrderWithoutErrors!.ShopifyOrderDetail!.Select(x => new CreateOrderRequestModel
    {
      StoreId = x.StoreId,
      OrderTypeId = x.OrderTypeId ?? (int)EnumOrderType.Regular,
      OrderDate = x.OrderDate,
      Description = x.Description,
      Remarks = x.Remarks,
      Amount = x.Amount,
      CShippingCharges = x.CShippingCharges,
      PaymentStatusId = x.PaymentStatusId,
      Weight = x.Weight,
      ItemValue = x.ItemValue,
      OrderRequestVia = x.OrderRequestVia ?? (int)EnumOrderRequestVia.Shopify,
      PaymentMethodId = x.PaymentMethodId,
      StationId = x.StationId,
      Discount = x.Discount,
      VAT = x.VAT,
      RefNo = x.RefNo,
      SaleChannelConfigId = x.SaleChannelConfigId,
      SaleChannelLookupId = x.SaleChannelLookupId,
      OrderNote = x.OrderNote != null ? new OrderNoteModel { Note = x.OrderNote.Note } : null,
      OrderAddress = x.OrderAddress != null ? new OrderAddressModel
      {
        CountryId = x.OrderAddress.Country,
        CityId = x.OrderAddress.City,
        AreaId = x.OrderAddress.Area,
        CustomerName = x.OrderAddress.CustomerName,
        Email = x.OrderAddress.Email,
        Mobile1 = x.OrderAddress.Mobile1,
        Mobile2 = x.OrderAddress.Mobile2,
        StreetAddress = x.OrderAddress.StreetAddress,
        StreetAddress2 = x.OrderAddress.StreetAddress2,
        PinCodeId = x.OrderAddress.PinCode,
        StateId = x.OrderAddress.State,
        HouseNo = x.OrderAddress.HouseNo,
        BuildingName = x.OrderAddress.BuildingName,
        Landmark = x.OrderAddress.Landmark,
        ProvinceId = x.OrderAddress.Province,
        AddressTypeId = (int)EnumAddressType.Shipping,
        Latitude = x.OrderAddress.Latitude,
        Longitude = x.OrderAddress.Longitude,

      } : null,
      OrderItems = x.OrderItems?.Select(item => new OrderItemModel
      {
        OrderItemId = item.OrderItemId,
        ProductId = item.ProductId,
        StockSku = item.StockSku,
        ProductVariantId = item.ProductVariantId,
        SaleChannelVariantId = item.SaleChannelVariantId,
        //price
        Price = item.Price,
        Description = item.Description,
        Remarks = item.Remarks,
        Quantity = item.Quantity,
        Discount = item.Discount,
      }).ToList(),
      OrderTaxes = x.OrderTaxes?.Select(tax => new OrderTaxModel
      {
        OrderTaxId = tax.OrderTaxId,
        ClientTaxId = tax.ClientTaxId,
        TaxValue = tax.TaxValue
      }).ToList()
    }).ToList();

    if (list.Count > 0)
    {
      var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.API, (int)EnumEnvironmentType.Dev);
      if (mcconfig is not null)
      {
        CreateOrderCommand createOrderCommand = new CreateOrderCommand();
        createOrderCommand.orderList = list;
        createOrderCommand.IsSaleChannelOrder = true; 
        var orderJson = JsonConvert.SerializeObject(createOrderCommand);
        string createdOrdersResponse = await _sharedCreateOrderShipraRepository.CreateOrder(orderJson, mcconfig!.Value!, oSaleChannelConfig.UserName, oSaleChannelConfig.Password);

        //if (!string.IsNullOrEmpty(createdOrdersResponse))
        //{
        //  var result = JsonConvert.DeserializeObject<ShipraControlPaneResponseModel<CreateOrderResponseModel>>(createdOrdersResponse);
        //  if (result != null && result!.isSuccess)
        //  {
        //    //do some work
        //    var createdOrders = result.result!.data;
        //    if (createdOrders!.Count > 0)
        //    {
        //      var trackingNos = string.Join(',', createdOrders.Select(x => x.OrderNo).ToList());
        //      List<Order> allOrderList = await _jobRepository.GetOrdersWithOrderNos(trackingNos, clientId!);

        //      foreach (var order in allOrderList)
        //      {
        //        #region MyRegion
        //        var orderNumber = !string.IsNullOrEmpty(order.RefNo!) ? int.Parse(order.RefNo!) : 0;
        //        var shopifyOrder = shopifyOrderList.Where(x => x.OrderNumber == orderNumber).FirstOrDefault();
        //        if (shopifyOrder is not null)
        //        {
        //          var shopifyJson = JsonConvert.SerializeObject(shopifyOrder);
        //          var createdDAte = ConvertDateTimeOffsetToDateTime(shopifyOrder!.CreatedAt!.GetValueOrDefault(), DateTimeConversionType.Utc);

        //          var oSaleChannelOrder = SaleChannelOrder.CreateSaleChannelOrder(oSaleChannelConfig.SaleChannelLookupId, oSaleChannelConfig.SaleChannelConfigId, shopifyOrder.Id!.ToString(), shopifyOrder.OrderNumber.ToString(), shopifyJson, createdDAte, order.OrderId!, clientId, employeeId);

        //          var isCreated = await _jobRepository.CreateSaleChannelOrder(oSaleChannelOrder, clientId);
        //        }
        //        #endregion
        //      }

        //    }
        //  }
        //}

      }
    }
    //foreach (var request in filtedredOrderWithoutErrors!.ShopifyOrderDetail!)
    //{
    //  // get order for check duplication
    //  var oSaleChannelOrder = await _jobRepository.GetSaleChannelOrderBySaleChannelNoByClient(request.RefNo, clientId);
    //  if (oSaleChannelOrder is null)
    //  {
    //    #region actual order place
    //    #region order address
    //    var objOrderAddress = request.OrderAddress!;
    //    if (!string.IsNullOrEmpty(objOrderAddress.Mobile1) && objOrderAddress.Mobile1.Contains("+"))
    //    {
    //      objOrderAddress.Mobile1 = RemoveWhitespace(objOrderAddress.Mobile1);
    //      objOrderAddress.Mobile1 = objOrderAddress.Mobile1.Replace("+", "00");
    //    }
    //    if (!string.IsNullOrEmpty(objOrderAddress.Mobile2) && objOrderAddress.Mobile2.Contains("+"))
    //    {
    //      objOrderAddress.Mobile2 = RemoveWhitespace(objOrderAddress.Mobile2!);
    //      objOrderAddress.Mobile2 = objOrderAddress.Mobile2.Replace("+", "00");
    //    }

    //    var orderAddress = OrderAddress.CreateOrderAddress(request.OrderAddress!.CustomerName, request.OrderAddress!.CustomerFullAddress, request.OrderAddress!.Email, request.OrderAddress!.Mobile1, request.OrderAddress!.Mobile2, request.OrderAddress!.Country, request.OrderAddress!.City, request.OrderAddress!.Area, request.OrderAddress!.StreetAddress, request.OrderAddress!.Latitude, request.OrderAddress!.Longitude, objOrderAddress!.StreetAddress2, objOrderAddress!.HouseNo, objOrderAddress!.BuildingName, objOrderAddress!.Landmark, objOrderAddress!.Province, objOrderAddress!.PinCode, objOrderAddress.State);

    //    var createdOrderAddress = await _jobRepository.CreateOrderAddress(orderAddress, clientId!);
    //    #endregion

    //    #region create order/tax
    //    var client = await _jobRepository.GetClientById(clientId);
    //    if (client is null)
    //    {
    //      throw new EntityNotFoundException("Client ", clientId.ToString());
    //    }
    //    var orderNo = await _jobRepository.GetClientNextOrderNo(clientId!);
    //    if (request.StoreId == 0)
    //    {
    //      request.StoreId = client!.DefaultStoreId;
    //    }
    //    if (request.StationId == 0)
    //    {
    //      request.StationId = client!.DefaultProductStationId.GetValueOrDefault();
    //    }
    //    if (request.PaymentMethodId == (int)EnumPaymentMethod.PP)
    //    {
    //      request.PaymentStatusId = (int)EnumPaymentStatus.Paid;
    //    }
    //    else
    //    {
    //      request.PaymentStatusId = (int)EnumPaymentStatus.Unpaid;
    //    }
    //    var totalTax = request.OrderTaxes!.Sum(x => x.TaxValue);
    //    request.ItemValue = request.Amount;
    //    var order = Order.CreateOrder(clientId!, request.StoreId, request.ChannelId, request.OrderTypeId, orderNo, request.OrderDate, orderAddress.OrderAddressId, request.Amount, request.Description, request.Remarks, request!.OrderItems?.Count(), request.CShippingCharges, request.PaymentStatusId, request.Weight, request.ItemValue, request.OrderRequestVia, request.Discount, totalTax.GetValueOrDefault(), request.PaymentMethodId, request.StationId, (int)EnumOrderOrderTrackingHistory.ORDERPLACED, request.RefNo, employeeId!);

    //    var createdOrder = await _jobRepository.CreateOrder(order, clientId!);


    //    #region order tax
    //    foreach (var item in request.OrderTaxes!)
    //    {
    //      var orderTax = OrderTax.Create(item.ClientTaxId, item.TaxValue, createdOrder.OrderId!);
    //      var added = await _jobRepository.CreateOrderTax(orderTax, clientId!);
    //    }
    //    #endregion
    //    #endregion

    //    #region order history/sale channel
    //    var createdByName = await _jobRepository.GetEmployeeNameById(employeeId, clientId!);

    //    var orderHistory = OrderTrackingHistory.CreateOrderTrackingHistory(createdOrder.OrderId!, (int)EnumOrderOrderTrackingHistory.ORDERPLACED, request.OrderNote?.Note, employeeId!, createdByName);
    //    var createdOrderTrackingHistory = await _jobRepository.CreateOrderTrackingHistory(orderHistory, clientId!);

    //    #region MyRegion
    //    var orderNumber = !string.IsNullOrEmpty(request.RefNo!) ? int.Parse(request.RefNo!) : 0;
    //    var shopifyOrder = shopifyOrderList.Where(x => x.OrderNumber == orderNumber).FirstOrDefault();
    //    var shopifyJson = JsonConvert.SerializeObject(shopifyOrder);
    //    var createdDAte = ConvertDateTimeOffsetToDateTime(shopifyOrder!.CreatedAt!.GetValueOrDefault(), DateTimeConversionType.Utc);

    //    oSaleChannelOrder = SaleChannelOrder.CreateSaleChannelOrder(oSaleChannelConfig.SaleChannelLookupId, oSaleChannelConfig.SaleChannelConfigId, request.SCOrderId!, request.SCOrderNo, shopifyJson, createdDAte, createdOrder.OrderId!, clientId, employeeId);

    //    var isCreated = await _jobRepository.CreateSaleChannelOrder(oSaleChannelOrder, clientId);
    //    #endregion
    //    #endregion

    //    #region orderitems
    //    foreach (var item in request!.OrderItems!)
    //    {
    //      await CreateOrderItemCommon(item, createdOrder, request.OrderTypeId, clientId, employeeId);
    //    }

    //    #endregion

    //    #region order note
    //    if (!string.IsNullOrEmpty(request?.OrderNote?.Note))
    //    {
    //      var createdOrderNote = await _jobRepository.CreateOrderNote(OrderNote.CreateOrderNote(order.OrderId!, request?.OrderNote?.Note, employeeId!), clientId);
    //    }
    //    #endregion
    //    #region order fulfill
    //    //when we create FullFilable Order 
    //    if (request!.OrderTypeId == (int)EnumOrderType.FullFilable)
    //    {
    //      // if user set autofullfill 
    //      if (isAutoFulfill)
    //      {
    //        foreach (var item in request!.OrderItems!)
    //        {
    //          dynamic histoy = new System.Dynamic.ExpandoObject();// { ReasonId = 0, Comment = string.Empty, PreviousQuantity = 0, NewQuantity = 0 };
    //          dynamic psQuantity = new System.Dynamic.ExpandoObject();// new { QuantityAvailable = 0, QuantityCommited = 0, QuantityOnOrder = 0 };

    //          var productStock = await _jobRepository.GetProductStockByIdAsync(item.ProductStockId.GetValueOrDefault(),clientId);
    //          if (item.Quantity <= productStock.QuantityAvailable)
    //          {
    //            //history for ProductStockAdjustment

    //            //previous qty
    //            histoy.PreviousQuantity = productStock!.QuantityAvailable;


    //            psQuantity.QuantityAvailable = productStock.QuantityAvailable - item.Quantity;
    //            var QtyCommited = productStock.QuantityCommited - item.Quantity;
    //            if (QtyCommited < 0)
    //            {
    //              psQuantity.QuantityCommited = 0;
    //            }
    //            else
    //            {
    //              psQuantity.QuantityCommited = QtyCommited;
    //            }
    //            psQuantity.QuantityOnOrder = productStock.QuantityOnOrder + item.Quantity;

    //            //new qty
    //            histoy.ReasonId = (int)EnumLookupAdjustReason.OrderFulfilled; //Adjust Stock
    //            histoy.Comment = $"Quantity Fullilled against {order.OrderNo}"; //Adjust Stock
    //            histoy.NewQuantity = psQuantity.QuantityAvailable;

    //            ///update and create product stock history
    //            await CreateProductStockHistoryAndUpdateProductStock(histoy, psQuantity, productStock,clientId,employeeId);

    //            order.UpdateOrderFulfillmentStatus((int)EnumFullfillmentStatus.Fulfilled,employeeId!, DateTime.UtcNow);
    //            await _jobRepository.UpdateOrder(order);
    //            #region order note 
    //            await _jobRepository.CreateOrderNote(OrderNote.CreateOrderNote(order.OrderId!, "Order Fulfilled through shopify", employeeId!),clientId);
    //            #endregion

    //          }

    //        }
    //        //order.UpdateOrderFulfillmentStatus((int)EnumFullfillmentStatus.Fulfilled, employeeId!, DateTime.UtcNow);
    //        //#region order note 
    //        //await _jobRepository.CreateOrderNote(OrderNote.CreateOrderNote(order.OrderId!, "Order fulfilled through Shopify service", employeeId!), clientId);
    //        //#endregion
    //      }
    //      else
    //      {
    //        order.UpdateOrderFulfillmentStatus((int)EnumFullfillmentStatus.Unfulfilled);
    //      }
    //    }
    //    #endregion
    //    #endregion
    //  }
    //  else
    //  {
    //    //update order if need
    //  }
    //}
  }
  #endregion
  //private async Task CreateProductStockHistoryAndUpdateProductStock(dynamic histoy, dynamic psQuantity, ProductStock? productStock, ClientId clientId, EmployeeId employeeId)
  //{
  //  #region update product stock 
  //  productStock!.UpdateFullfilmentStock(psQuantity.QuantityAvailable, psQuantity.QuantityCommited, psQuantity.QuantityOnOrder, employeeId!);
  //  var updatedProdctStock = await _jobRepository.UpdateProductStockAsync(productStock, clientId);
  //  //update low quantity
  //  //productStock.UpdateLoWQuantityLimit(psQuantity.QuantityAvailable, productStock.LowQuantityLimit);
  //  #endregion
  //  #region product stock history 
  //  ///create stock history and update oreer
  //  ProductStockHistory stockHistory = ProductStockHistory.CreateProductStockHistory(histoy.ReasonId, productStock.ProductStockId, histoy.PreviousQuantity, histoy.NewQuantity, histoy.Comment, employeeId!);
  //  dynamic addedHisotry = await _jobRepository.CreateProductStockHistory(stockHistory, clientId);
  //  #endregion
  //}
  #endregion


  #region Shopify
  private ShopifyOrderDetailResponse FilterOrdersWithNoErrors(ShopifyOrderDetailResponse response)
  {
    // Initialize a new response object to store the filtered results
    var filteredResponse = new ShopifyOrderDetailResponse
    {
      ShopifyOrderDetail = new List<CreateUploadOrderResponseModel>(),
      Errors = new List<ShopifyOrderError>(),
      IsSuccessed = response.IsSuccessed
    };

    // Filter the records where the error messages have count 0
    for (var i = 0; i < response.Errors?.Count; i++)
    {
      if (response.Errors[i].Msg.Count == 0)
      {
        // Add corresponding ShopifyOrderDetail and Errors entries to the new response
        filteredResponse.ShopifyOrderDetail?.Add(response.ShopifyOrderDetail?[i]!);
        filteredResponse.Errors?.Add(response.Errors[i]);
      }
    }

    return filteredResponse;
  }
  private async Task<List<ShopifySharp.Order>> GetAllOrdersListOnShopForShopify(string shopDomain, string accesstoken, DateTimeOffset createdFrom, DateTimeOffset createdTo)
  {
    shopDomain = "demoimepress.myshopify.com";
    accesstoken = "shpua_489b8265d3732d74aa797c537fc9dffc";
    var executionPolicy = new LeakyBucketExecutionPolicy();
    var service = new OrderService(shopDomain, accesstoken);
    var allOrders = new List<ShopifySharp.Order>();

    var page = await service.ListAsync(new OrderListFilter
    {
      Limit = 250,
      CreatedAtMin = createdFrom,
      CreatedAtMax = createdTo,

    });
    // Keep adding the orders to the list of all orders until there are no

    while (true)
    {
      allOrders.AddRange(page.Items);
      if (!page.HasNextPage)
      {
        // We've reached the end of the list
        break;
      }
      // There is at least one more page, list it and loop again
      page = await service.ListAsync(page.GetNextPageFilter());
    }
    return allOrders;
    // TODO: do something with the `allOrders` variable
  }
  private async Task<List<ShopifySharp.Order>> GetAllOrdersListOnShopForShopifyWithoutDateFilter(string shopDomain, string accesstoken)
  {
    shopDomain = "demoimepress.myshopify.com";
    accesstoken = "shpua_489b8265d3732d74aa797c537fc9dffc";
    var executionPolicy = new LeakyBucketExecutionPolicy();
    var service = new OrderService(shopDomain, accesstoken);
    var allOrders = new List<ShopifySharp.Order>();

    var page = await service.ListAsync(new OrderListFilter
    {
      Limit = 250,
      CreatedAtMin = null,
      CreatedAtMax = null,
    });
    // Keep adding the orders to the list of all orders until there are no

    while (true)
    {
      allOrders.AddRange(page.Items);
      if (!page.HasNextPage)
      {
        // We've reached the end of the list
        break;
      }
      // There is at least one more page, list it and loop again
      page = await service.ListAsync(page.GetNextPageFilter());
    }
    return allOrders;
    // TODO: do something with the `allOrders` variable
  }

  #endregion

  public async Task CreateSaleChannelOrderById(List<CreateOrderResponseDetailModel>? createdOrders, CreateSaleChannelRequestModel createSaleChannel)
  {
    if (createdOrders is not null && createdOrders.Count > 0)
    {
      var oShopifyConfig = createSaleChannel.ShopifyConfig!;
      var oSaleChannelConfig = createSaleChannel.SaleChannelConfig!;
      var clientId = createSaleChannel.ClientId!;
      var employeeId = createSaleChannel.EmployeeId!;

      var shopifyOrderList = await GetAllOrdersListOnShopForShopifyWithoutDateFilter(oShopifyConfig.ShopDomain!, oShopifyConfig.AccessToken!);

      var trackingNos = string.Join(',', createdOrders.Select(x => x.OrderNo).ToList());
      List<Order> allOrderList = await _jobRepository.GetOrdersWithOrderNos(trackingNos, clientId!);

      foreach (var order in allOrderList)
      {
        #region MyRegion
        var orderNumber = !string.IsNullOrEmpty(order.RefNo!) ? int.Parse(order.RefNo!) : 0;
        var shopifyOrder = shopifyOrderList.Where(x => x.OrderNumber == orderNumber).FirstOrDefault();
        if (shopifyOrder is not null)
        {
          var shopifyJson = JsonConvert.SerializeObject(shopifyOrder);
          var createdDAte = ConvertDateTimeOffsetToDateTime(shopifyOrder!.CreatedAt!.GetValueOrDefault(), DateTimeConversionType.Utc);

          var oSaleChannelOrder = SaleChannelOrder.CreateSaleChannelOrder(oSaleChannelConfig.SaleChannelLookupId, oSaleChannelConfig.SaleChannelConfigId, shopifyOrder.Id!.ToString(), shopifyOrder.OrderNumber.ToString(), shopifyJson, createdDAte, order.OrderId!, clientId, employeeId);

          var isCreated = await _jobRepository.CreateSaleChannelOrder(oSaleChannelOrder, clientId);
        }
        #endregion
      }
    }
  }
}
