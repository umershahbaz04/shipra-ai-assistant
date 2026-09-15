using DocumentFormat.OpenXml.Drawing;
using Hangfire;
using Hangfire.Storage;
using Hangfire.Storage.Monitoring;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.DTOs.ClientUseCase.Response;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Application.DTOs.SaleChannelUseCase.ShopifyOrder;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrder;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Application.Services.Implementation.Modified;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.ProductAggregate;
using Shipra.Backend.API.Core.SaleChannelConfigAggregate;
using Shipra.Backend.API.Core.SaleChannelOrderAggregate;
using Shipra.Backend.API.Core.StoresAggregate;
using Shipra.Backend.API.SharedKernel.Interfaces;
using Shipra.Backend.API.SharedKernel.Models;
using ShopifySharp;
using ShopifySharp.Filters;
using ShopifySharp.GraphQL;
using static Shipra.Backend.API.Application.Helpers.Utility;
using InventoryLevel = ShopifySharp.InventoryLevel;
using Order = Shipra.Backend.API.Core.OrderAggregate.Order;
using Province = Shipra.Backend.API.Core.CountryAggregate.Province;

namespace Shipra.Backend.API.Application.Services.Implementation;
//public class SaleChannelOrderService : ISaleChannelOrderService
//{
//  private readonly IJobRepository _jobRepository;
//  private readonly ISharedClientRepository _sharedClientRepository;
//  private readonly IConfigRepository _configRepository;

//  private readonly ICountryRepository _countryRepository;

//  public SaleChannelOrderService(IJobRepository jobRepository, ISharedClientRepository sharedClientRepository, IConfigRepository configRepository, ICountryRepository countryRepository)
//  {
//    _jobRepository = jobRepository;
//    _sharedClientRepository = sharedClientRepository;
//    _configRepository = configRepository;
//    _countryRepository = countryRepository;
//  }

//  public async Task SaleChannelsOrderManageQueuesAsync()
//  {
//    var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.AdminControlPanKey, (int)EnumEnvironmentType.Dev);
//    if (mcconfig is null)
//    {
//      throw new EntityNotFoundException("Mcconfig", "Admin ControlPan Value");
//    }
//    var result = await _sharedClientRepository.GetAllClients(mcconfig.Value!);
//    if (!string.IsNullOrEmpty(result))
//    {
//      var deseralisedResponse = JsonConvert.DeserializeObject<AuthResponseModel<List<ClientSummaryResponseModel>>>(result);

//      var saleChannelConfig = await _jobRepository.GetAllSaleChannelsForJob(deseralisedResponse!.result!);
//      #region static
//      var countries = await _countryRepository.GetAllCountries();
//      var cities = await _countryRepository.GetAllCities();
//      var provinces = await _countryRepository.GetAllProvinces();
//      var states = await _countryRepository.GetAllStates();
//      var areas = await _countryRepository.GetAllAreas();
//      var pinCodes = await _countryRepository.GetAllPinCodes();
//      #endregion
//      // Example: Replace with actual productId, variantId, and the new quantity
//      //long productId = 8459584602269;   // Replace with actual product ID
//      //long variantId = 46005205696669;   // Replace with actual variant ID
//      //int newQuantity = 50;          // New inventory quantity

//      //await UpdateInventoryAsync("", "", productId, variantId, newQuantity);

//      //group by SaleChannelLookupId for better identification
//      var groupSaleLookups = saleChannelConfig.Where(x => x.SaleChannelLookupId != (int)EnumSaleChannelLookup.SalePerson).GroupBy(x => x.SaleChannelLookupId);
//      foreach (var oSaleChannelLookups in groupSaleLookups)
//      {
//        // group by client for client id 
//        //var allSameSalechannelByClients = saleChannelConfig.Where(x => x.SaleChannelConfigId == saleChannelConfigId).GroupBy(x => x.ClientId);

//        foreach (var oSaleChannelsResponse in oSaleChannelLookups!)
//        {
//          string orderType = Utils.GetClientSettingValueWithByKey(oSaleChannelsResponse.SCSettingConfig, "order", "orderType");
//          string autoFulfill = Utils.GetClientSettingValueWithByKey(oSaleChannelsResponse.SCSettingConfig, "order", "autoFulfill");
//          // Convert to bool using TryParse
//          bool isAutoFulfill;
//          bool.TryParse(autoFulfill, out isAutoFulfill);

//          var oSaleChannelConfig = new SaleChannelConfig();
//          oSaleChannelConfig.SaleChannelConfigId = oSaleChannelsResponse.SaleChannelConfigId;
//          oSaleChannelConfig.SaleChannelLookupId = oSaleChannelsResponse.SaleChannelLookupId;
//          oSaleChannelConfig.StoreId = oSaleChannelsResponse.StoreId;

//          int? saleChannelConfigId = oSaleChannelConfig.SaleChannelConfigId;

//          var clientId = new ClientId(oSaleChannelsResponse!.ClientId!.Value);
//          var employeeId = new EmployeeId(oSaleChannelsResponse!.ClientId!.Value);
//          var oStore = await _jobRepository.GetStoreById(oSaleChannelConfig.StoreId.GetValueOrDefault(), clientId!);

//          if (oSaleChannelsResponse.SaleChannelLookupId == (int)EnumSaleChannelLookup.Shopify)
//          {
//            //1. Validate to get shopify config
//            var oShopifyConfig = await _jobRepository.GetShopifyConfigByClientId(saleChannelConfigId, clientId);
//            if (oShopifyConfig is not null && oShopifyConfig.AccessToken is not null)
//            {
//              //2. Fetch Orders data from shopify
//              var sdate = DateTime.Now.AddDays(-30);
//              var shopifyOrderList = await GetAllOrdersListOnShopForShopify(oShopifyConfig.ShopDomain!, oShopifyConfig.AccessToken!, ConvertUTCDateToDateTimeOffSet(DateTime.UtcNow.AddDays(-30)), ConvertUTCDateToDateTimeOffSet(DateTime.UtcNow));
//              if (shopifyOrderList is not null && shopifyOrderList.Count > 0)
//              {
//                //3. Convert new order data OrderId to String (,) Ids
//                var shopifyOrderIds = string.Join(',', shopifyOrderList.Select(x => x.Id));

//                //4. Get record from existing Shipra sale order data to compare with new shopify order list
//                var oSaleChannelOrderList = await _jobRepository.GetSaleChannelOrderListByOrderIds(shopifyOrderIds, (int)EnumSaleChannelLookup.Shopify, clientId!);

//                //dynamic data;

//                if (oSaleChannelOrderList is not null && oSaleChannelOrderList.Count > 0)
//                {
//                  //5 If order found in sale channel Order Data then first get orderIds
//                  var saleChannelOrderIds = oSaleChannelOrderList.Select(x => x.OrderId!).ToList();

//                  //6 Convert the list of strings to a list of long values using LINQ
//                  var longList = saleChannelOrderIds.Select(s => long.Parse(s!)).ToList();

//                  //7. Discard those record which already exist
//                  shopifyOrderList = shopifyOrderList.Where(x => !longList!.Contains((long)x.Id!)).ToList();
//                }
//                //Filter those oders from list have no customer address and zero Item count
//                shopifyOrderList = shopifyOrderList.Where(x => x.ShippingAddress != null && x.ShippingAddress.Name != null && x.ShippingAddress.Address1 != null && x.LineItems.Count() > 0).ToList();
//                if (orderType == EnumOrderType.Regular.ToString())
//                {
//                  //concider all order as regular
//                  await CreateShopifyOrder123(shopifyOrderList, oStore!, countries, cities, provinces, states, areas, pinCodes, oSaleChannelConfig, (int)EnumOrderType.Regular, clientId, employeeId, isAutoFulfill);

//                  // Schedule a recurring job that runs every 5 minutes
//                  RecurringJob.AddOrUpdate(
//                      $"{oSaleChannelsResponse.SaleChannelName}_RecurringJob_{orderType}",            // A unique identifier for the recurring job
//                      () => RecurringJobMethod(mcconfig.Value!),              // Method to call
//                     "*/1 * * * *"                          // Cron expression for every 5 minutes
//                  );
//                }
//                else if (orderType == EnumOrderType.FullFilable.ToString())
//                {
//                  //Filter those oders from list have  SKUs
//                  shopifyOrderList = shopifyOrderList.Where(x => x.LineItems.Any(v => v.SKU != null && v.SKU != "")).ToList();
//                  await CreateShopifyOrder123(shopifyOrderList, oStore!, countries, cities, provinces, states, areas, pinCodes, oSaleChannelConfig, (int)EnumOrderType.FullFilable, clientId, employeeId, isAutoFulfill);
//                }
//                else if (orderType == EnumOrderType.Both.ToString())
//                {
//                  var shopifyOrderListForRegular = shopifyOrderList.Where(x => x.LineItems.Any(v => v.SKU == null && v.VariantId == null)).ToList();
//                  if (shopifyOrderListForRegular.Count > 0)
//                  {
//                    await CreateShopifyOrder123(shopifyOrderListForRegular, oStore!, countries, cities, provinces, states, areas, pinCodes, oSaleChannelConfig, (int)EnumOrderType.Regular, clientId, employeeId, isAutoFulfill);
//                  }

//                  //Filter those oders from list have  SKUs
//                  var shopifyOrderListForFulillable = shopifyOrderList.Where(x => x.LineItems.Any(v => v.SKU != null && v.SKU != "")).ToList();
//                  if (shopifyOrderListForFulillable.Count > 0)
//                  {
//                    await CreateShopifyOrder123(shopifyOrderListForFulillable, oStore!, countries, cities, provinces, states, areas, pinCodes, oSaleChannelConfig, (int)EnumOrderType.FullFilable, clientId, employeeId, isAutoFulfill);
//                  }
//                }
//              }
//              else
//              {
//                continue;
//              }
//            }
//            else
//            {
//              continue;
//            }
//          }
//          else if (oSaleChannelsResponse.SaleChannelLookupId == (int)EnumSaleChannelLookup.WooCommerce)
//          {
//            //woocomerece code here
//          }

//        }
//      }
//      //// Schedule a recurring job that runs every 5 minutes
//      // RecurringJob.AddOrUpdate(
//      //     "RecurringJob_Every1Minutes",            // A unique identifier for the recurring job
//      //     () => RecurringJobMethod(mcconfig.Value!),              // Method to call
//      //    "*/1 * * * *"                          // Cron expression for every 5 minutes
//      // ); 
//    }
//  }
//  #region regular orer
//  public async Task CreateOrderItemCommon(OrderItemModel item, Order createdOrder, int? orderTypeId, ClientId clientId, EmployeeId employeeId)
//  {
//    OrderItem? orderItem = null;
//    if (orderTypeId == (int)EnumOrderType.FullFilable)
//    {
//      var product = await _jobRepository.GetProductByIdAsync(new ProductId(new Guid(item.ProductId!)), clientId!);
//      if (product is null)
//      {
//        if (product!.TrackInventory.GetValueOrDefault())
//        {
//          //with product inventory
//          #region update productstock
//          var productStock = await _jobRepository.GetProductStockByIdAsync(item.ProductStockId.GetValueOrDefault(), clientId);
//          if (productStock is not null)
//          {
//            var quantityCommited = productStock!.QuantityCommited + item.Quantity;
//            productStock.UpdateProductStockQuantityCommited(quantityCommited, employeeId!);
//            ProductStock updatedProductStock = await _jobRepository.UpdateProductStockAsync(productStock, clientId);
//            #endregion

//            item.Description = OrderCommon.GetDescriptionForShipmentItem(product, productStock);

//            orderItem = OrderItem.CreateOrderItemFullFilable(createdOrder.OrderId!, new ProductId(new Guid(item.ProductId!)), item.ProductStockId, item.Price, item.Description, item.Remarks, item.Quantity, item.Discount);
//            OrderItem createdItem = await _jobRepository.CreateOrderItem(orderItem!, clientId);
//          }
//        }
//        else
//        {
//          ///without  inventory stock id will be 0
//          item.ProductStockId = null;
//          orderItem = OrderItem.CreateOrderItemFullFilable(createdOrder.OrderId!, new ProductId(new Guid(item.ProductId!)), item.ProductStockId, item.Price, item.Description, item.Remarks, item.Quantity, item.Discount);
//          OrderItem createdItem = await _jobRepository.CreateOrderItem(orderItem!, clientId);
//        }
//      }
//    }
//    else if (orderTypeId == (int)EnumOrderType.Regular)
//    {
//      orderItem = OrderItem.CreateOrderItemRegular(createdOrder.OrderId!, item.Price, item.Description, item.Remarks, item.Quantity, item.Discount);
//      OrderItem createdItem = await _jobRepository.CreateOrderItem(orderItem!, clientId);
//    }
//  }

//  public async Task CreateShopifyOrder123(
//    List<ShopifySharp.Order> shopifyOrderList,
//    Store oStore,
//    List<Core.CountryAggregate.Country> countries,
//    List<City> cities,
//    List<Province> provinces,
//    List<State> states,
//    List<Area> areas,
//    List<PinCode> pinCodes,
//    SaleChannelConfig oSaleChannelConfig,
//    int orderTypeId,
//    ClientId clientId,
//    EmployeeId employeeId, bool isAutoFulfill)
//  {
//    //Filter and select shopify orders line items SKUs to match with Shipra
//    var shopifyProductSKUsList = shopifyOrderList!.SelectMany(order => order.LineItems).Select(lineItem => lineItem.SKU);
//    //Join list to comman seprated string
//    var shopifyProductSKUs = string.Join(",", shopifyProductSKUsList);
//    //Get all match SKUs product from shipra product stock
//    var oShipraProductList = await _jobRepository.GetAllProductStocksForSaleChannelInventorySync(clientId!.Value.ToString()!, shopifyProductSKUs);
//    var castedList = (IEnumerable<dynamic>)oShipraProductList!;
//    //Filter Shipra Product SKUs List
//    var productSKUs = castedList!.Select(p => p.SKU).ToList();
//    //Filter record from shipra order list get only dose oders match with Shipra stock
//    var matchedOrders = shopifyOrderList.Where(order => order.LineItems.Any(item => productSKUs.Contains(item.SKU))).ToList();

//    ShopifyOrderDetailResponse response = ShopifyOrderDetailSimplified.ConverShopifytoShipraOrderDetail(matchedOrders, countries, cities, provinces, states, areas, pinCodes, oStore, (int)EnumStationLookup.Dubai, oSaleChannelConfig, (int)EnumSaleChannelLookup.Shopify, orderTypeId, castedList);

//    var filtedredOrderWithoutErrors = FilterOrdersWithNoErrors(response);


//    foreach (var request in filtedredOrderWithoutErrors!.ShopifyOrderDetail!)
//    {
//      // get order for check duplication
//      var oSaleChannelOrder = await _jobRepository.GetSaleChannelOrderBySaleChannelNoByClient(request.RefNo, clientId);
//      if (oSaleChannelOrder is null)
//      {
//        #region actual order place
//        #region order address
//        var objOrderAddress = request.OrderAddress!;
//        if (!string.IsNullOrEmpty(objOrderAddress.Mobile1) && objOrderAddress.Mobile1.Contains("+"))
//        {
//          objOrderAddress.Mobile1 = RemoveWhitespace(objOrderAddress.Mobile1);
//          objOrderAddress.Mobile1 = objOrderAddress.Mobile1.Replace("+", "00");
//        }
//        if (!string.IsNullOrEmpty(objOrderAddress.Mobile2) && objOrderAddress.Mobile2.Contains("+"))
//        {
//          objOrderAddress.Mobile2 = RemoveWhitespace(objOrderAddress.Mobile2!);
//          objOrderAddress.Mobile2 = objOrderAddress.Mobile2.Replace("+", "00");
//        }

//        var orderAddress = OrderAddress.CreateOrderAddress(request.OrderAddress!.CustomerName, request.OrderAddress!.CustomerFullAddress, request.OrderAddress!.Email, request.OrderAddress!.Mobile1, request.OrderAddress!.Mobile2, request.OrderAddress!.Country, request.OrderAddress!.City, request.OrderAddress!.Area, request.OrderAddress!.StreetAddress, request.OrderAddress!.Latitude, request.OrderAddress!.Longitude, objOrderAddress!.StreetAddress2, objOrderAddress!.HouseNo, objOrderAddress!.BuildingName, objOrderAddress!.Landmark, objOrderAddress!.Province, objOrderAddress!.PinCode, objOrderAddress.State);

//        var createdOrderAddress = await _jobRepository.CreateOrderAddress(orderAddress, clientId!);
//        #endregion

//        #region create order
//        var client = await _jobRepository.GetClientById(clientId);
//        if (client is null)
//        {
//          throw new EntityNotFoundException("Client ", clientId.ToString());
//        }
//        var orderNo = await _jobRepository.GetClientNextOrderNo(clientId!);
//        if (request.StoreId == 0)
//        {
//          request.StoreId = client!.DefaultStoreId;
//        }
//        if (request.StationId == 0)
//        {
//          request.StationId = client!.DefaultProductStationId.GetValueOrDefault();
//        }
//        if (request.PaymentMethodId == (int)EnumPaymentMethod.PP)
//        {
//          request.PaymentStatusId = (int)EnumPaymentStatus.Paid;
//        }
//        else
//        {
//          request.PaymentStatusId = (int)EnumPaymentStatus.Unpaid;
//        }
//        var totalTax = request.OrderTaxes!.Sum(x => x.TaxValue);
//        request.ItemValue = request.Amount;
//        var order = Order.CreateOrder(clientId!, request.StoreId, request.ChannelId, request.OrderTypeId, orderNo, request.OrderDate, orderAddress.OrderAddressId, request.Amount, request.Description, request.Remarks, request!.OrderItems?.Count(), request.CShippingCharges, request.PaymentStatusId, request.Weight, request.ItemValue, request.OrderRequestVia, request.Discount, totalTax.GetValueOrDefault(), request.PaymentMethodId, request.StationId, (int)EnumOrderOrderTrackingHistory.ORDERPLACED, request.RefNo, employeeId!);

//        //when we create FullFilable Order 
//        if (request.OrderTypeId == (int)EnumOrderType.FullFilable)
//        {
//          if (isAutoFulfill)
//          {
//            order.UpdateOrderFulfillmentStatus((int)EnumFullfillmentStatus.Fulfilled, employeeId!, DateTime.UtcNow);
//            #region order note 
//            await _jobRepository.CreateOrderNote(OrderNote.CreateOrderNote(order.OrderId!, "Order fulfilled through Shopify service", employeeId!), clientId);
//            #endregion
//          }
//          else
//          {
//            order.UpdateOrderFulfillmentStatus((int)EnumFullfillmentStatus.Unfulfilled);
//          }
//        }

//        var createdOrder = await _jobRepository.CreateOrder(order, clientId!);


//        #region order tax
//        foreach (var item in request.OrderTaxes!)
//        {
//          var orderTax = OrderTax.Create(item.ClientTaxId, item.TaxValue, createdOrder.OrderId!);
//          var added = await _jobRepository.CreateOrderTax(orderTax, clientId!);
//        }
//        #endregion
//        #endregion

//        #region order history
//        var createdByName = await _jobRepository.GetEmployeeNameById(employeeId, clientId!);

//        var orderHistory = OrderTrackingHistory.CreateOrderTrackingHistory(createdOrder.OrderId!, (int)EnumOrderOrderTrackingHistory.ORDERPLACED, request.OrderNote?.Note, employeeId!, createdByName);
//        var createdOrderTrackingHistory = await _jobRepository.CreateOrderTrackingHistory(orderHistory, clientId!);

//        #region MyRegion
//        var orderNumber = !string.IsNullOrEmpty(request.RefNo!) ? int.Parse(request.RefNo!) : 0;
//        var shopifyOrder = shopifyOrderList.Where(x => x.OrderNumber == orderNumber).FirstOrDefault();
//        var shopifyJson = JsonConvert.SerializeObject(shopifyOrder);
//        var createdDAte = ConvertDateTimeOffsetToDateTime(shopifyOrder!.CreatedAt!.GetValueOrDefault(), DateTimeConversionType.Utc);

//        oSaleChannelOrder = SaleChannelOrder.CreateSaleChannelOrder(oSaleChannelConfig.SaleChannelLookupId, oSaleChannelConfig.SaleChannelConfigId, request.SCOrderId!, request.SCOrderNo, shopifyJson, createdDAte, createdOrder.OrderId!, clientId, employeeId);

//        var isCreated = await _jobRepository.CreateSaleChannelOrder(oSaleChannelOrder, clientId);
//        #endregion
//        #endregion

//        #region orderitems
//        foreach (var item in request!.OrderItems!)
//        {
//          await CreateOrderItemCommon(item, createdOrder, request.OrderTypeId, clientId, employeeId);
//        }

//        #endregion

//        #region order note
//        if (!string.IsNullOrEmpty(request?.OrderNote?.Note))
//        {
//          var createdOrderNote = await _jobRepository.CreateOrderNote(OrderNote.CreateOrderNote(order.OrderId!, request?.OrderNote?.Note, employeeId!), clientId);
//        }
//        #endregion
//        #endregion
//      }
//      else
//      {
//        //update order if need
//      }
//    }
//  }
//  #endregion
//  public void RecurringJobMethod(string val)
//  {
//    Console.WriteLine("Recurring job executed every 5 minutes");
//  }

//  #region Shopify
//  public ShopifyOrderDetailResponse FilterOrdersWithNoErrors(ShopifyOrderDetailResponse response)
//  {
//    // Initialize a new response object to store the filtered results
//    var filteredResponse = new ShopifyOrderDetailResponse
//    {
//      ShopifyOrderDetail = new List<CreateUploadOrderResponseModel>(),
//      Errors = new List<ShopifyOrderError>(),
//      IsSuccessed = response.IsSuccessed
//    };

//    // Filter the records where the error messages have count 0
//    for (var i = 0; i < response.Errors?.Count; i++)
//    {
//      if (response.Errors[i].Msg.Count == 0)
//      {
//        // Add corresponding ShopifyOrderDetail and Errors entries to the new response
//        filteredResponse.ShopifyOrderDetail?.Add(response.ShopifyOrderDetail?[i]!);
//        filteredResponse.Errors?.Add(response.Errors[i]);
//      }
//    }

//    return filteredResponse;
//  }
//  public async Task<List<ShopifySharp.Order>> GetAllOrdersListOnShopForShopify(string shopDomain, string accesstoken, DateTimeOffset createdFrom, DateTimeOffset createdTo)
//  {
//    shopDomain = "demoimepress.myshopify.com";
//    accesstoken = "shpua_489b8265d3732d74aa797c537fc9dffc";
//    var executionPolicy = new LeakyBucketExecutionPolicy();
//    var service = new OrderService(shopDomain, accesstoken);
//    var allOrders = new List<ShopifySharp.Order>();

//    var page = await service.ListAsync(new OrderListFilter
//    {
//      Limit = 250,
//      CreatedAtMin = createdFrom,
//      CreatedAtMax = createdTo,
//    });
//    // Keep adding the orders to the list of all orders until there are no

//    while (true)
//    {
//      allOrders.AddRange(page.Items);
//      if (!page.HasNextPage)
//      {
//        // We've reached the end of the list
//        break;
//      }
//      // There is at least one more page, list it and loop again
//      page = await service.ListAsync(page.GetNextPageFilter());
//    }
//    return allOrders;
//    // TODO: do something with the `allOrders` variable
//  }
//  #endregion


//  public async Task UpdateInventoryAsync(string _shopUrl, string _accessToken, long productId, long variantId, int newQuantity)
//  {
//    _shopUrl = "demoimepress.myshopify.com";
//    _accessToken = "shpua_489b8265d3732d74aa797c537fc9dffc";
//    var executionPolicy = new LeakyBucketExecutionPolicy();
//    try
//    {
//      // Fetch the product variant to get the InventoryItemId
//      var variantService = new ProductVariantService(_shopUrl, _accessToken);
//      var variant = await variantService.GetAsync(variantId);
//      long inventoryItemId = variant.InventoryItemId.GetValueOrDefault();

//      // Fetch all locations and select the first one (you can choose based on your logic)
//      var locationService = new LocationService(_shopUrl, _accessToken);
//      var locations = await locationService.ListAsync();
//      long? locationId = locations!.Items!.FirstOrDefault()!.Id;

//      // Update the inventory level for the product variant
//      var inventoryLevelService = new InventoryLevelService(_shopUrl, _accessToken);
//      InventoryLevel obj = new InventoryLevel
//      {
//        LocationId = locationId,
//        InventoryItemId = inventoryItemId,
//        Available = newQuantity,
//      };
//      var inventoryLevel = await inventoryLevelService.SetAsync(obj);

//      Console.WriteLine($"Inventory updated successfully. New quantity at location {locationId}: {inventoryLevel.Available}");
//    }
//    catch (ShopifyException ex)
//    {
//      Console.WriteLine($"Error updating inventory: {ex.Message}");
//    }
//  }
//}
//===============================================================
//public class SaleChannelOrderService : ISaleChannelOrderService
//{
//  private readonly ISaleChannelFactory _saleChannelFactory;
//  private readonly IJobRepository _jobRepository;
//  private readonly ISharedClientRepository _sharedClientRepository;
//  private readonly IConfigRepository _configRepository;

//  public SaleChannelOrderService(ISaleChannelFactory saleChannelFactory, IJobRepository jobRepository, ISharedClientRepository sharedClientRepository, IConfigRepository configRepository)
//  {
//    _saleChannelFactory = saleChannelFactory;
//    _jobRepository = jobRepository;
//    _sharedClientRepository = sharedClientRepository;
//    _configRepository = configRepository;
//  }

//  public async Task SaleChannelsOrderManageQueuesAsync()
//  {
//    var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.AdminControlPanKey, (int)EnumEnvironmentType.Live);
//    if (mcconfig is not null)
//    {
//      var result = await _sharedClientRepository.GetAllClients(mcconfig!.Value!);
//      if (!string.IsNullOrEmpty(result))
//      {
//        var deserializedResponse = JsonConvert.DeserializeObject<AuthResponseModel<List<ClientSummaryResponseModel>>>(result);


//        var saleChannelConfig = await _jobRepository.GetAllSaleChannelsForJob(deserializedResponse!.result!);
//        //get all sale channels like, shopify/woocomerecec etc
//        var groupedSaleLookups = saleChannelConfig.GroupBy(x => x.SaleChannelLookupId);
//        // list of channel like shopify/woocomere
//        foreach (var saleChannelLookupGroup in groupedSaleLookups)
//        {
//          // run job for each channel like for shopify,woocomere and any other channel each new channel have its own job wich will rerun after 10 minut
//          foreach (var oSaleChannelsResponse in saleChannelLookupGroup)
//          {
//            string orderType = Utils.GetClientSettingValueWithByKey(oSaleChannelsResponse.SCSettingConfig, "order", "orderType");
//            string autoFulfill = Utils.GetClientSettingValueWithByKey(oSaleChannelsResponse.SCSettingConfig, "order", "autoFulfill");

//            orderType = !string.IsNullOrEmpty(orderType) ? orderType.Trim().ToLower() : string.Empty;
//            // Convert to bool using TryParse
//            bool isAutoFulfill;
//            bool.TryParse(autoFulfill, out isAutoFulfill);

//            var oSaleChannelConfig = new SaleChannelConfig();
//            oSaleChannelConfig.SaleChannelConfigId = oSaleChannelsResponse.SaleChannelConfigId;
//            oSaleChannelConfig.SaleChannelLookupId = oSaleChannelsResponse.SaleChannelLookupId;
//            oSaleChannelConfig.StoreId = oSaleChannelsResponse.StoreId;
//            oSaleChannelConfig.SaleChannelName = oSaleChannelsResponse.SaleChannelName;

//            oSaleChannelConfig.UserName = oSaleChannelsResponse.UserName;
//            oSaleChannelConfig.Password = oSaleChannelsResponse.Password;
//            if (!string.IsNullOrEmpty(oSaleChannelConfig.UserName) && !string.IsNullOrEmpty(oSaleChannelConfig.Password))
//            {
//              var saleChannelConfigId = oSaleChannelsResponse.SaleChannelConfigId;
//              var clientId = new ClientId(oSaleChannelsResponse!.ClientId!.Value);
//              var employeeId = new EmployeeId(oSaleChannelsResponse!.ClientId!.Value);

//              var oStore = await _jobRepository.GetStoreById(oSaleChannelConfig.StoreId.GetValueOrDefault(), clientId!);

//              var saleChannelService = _saleChannelFactory.GetSaleChannelService(oSaleChannelsResponse.SaleChannelLookupId.GetValueOrDefault());
//              //await saleChannelService.ProcessOrdersAsync(saleChannelConfigId, clientId);
//              // Create a dynamic job name based on SaleChannelLookupId 
//              // Create a dynamic job name based on SaleChannelLookupId
//              var jobName = $"{oSaleChannelConfig.SaleChannelName}_OrderProcessing";

//              await saleChannelService.ProcessOrdersAsync(saleChannelConfigId, clientId, oStore!, oSaleChannelConfig, oSaleChannelsResponse, orderType, employeeId, isAutoFulfill);
//              // Schedule a recurring job for each sale channel
//              RecurringJob.AddOrUpdate(
//                  jobName,
//                  () => saleChannelService.ProcessOrdersAsync(saleChannelConfigId, clientId, oStore!, oSaleChannelConfig, oSaleChannelsResponse, orderType, employeeId, isAutoFulfill),
//                  "*/1 * * * *" // Every 5 minutes
//              );
//            }
//          }
//        }

//      }
//    }
//  }
//}
//=============================leatest code==============
public class SaleChannelOrderService : ISaleChannelOrderService
{
  private readonly ISaleChannelFactory _saleChannelFactory;
  private readonly IJobRepository _jobRepository;
  private readonly ISharedClientRepository _sharedClientRepository;
  private readonly IConfigRepository _configRepository;

  public SaleChannelOrderService(ISaleChannelFactory saleChannelFactory, IJobRepository jobRepository, ISharedClientRepository sharedClientRepository, IConfigRepository configRepository)
  {
    _saleChannelFactory = saleChannelFactory;
    _jobRepository = jobRepository;
    _sharedClientRepository = sharedClientRepository;
    _configRepository = configRepository;
  }
  public async Task ScheduleMainSaleChannelOrderJob()
  {
    // Schedule the SaleChannelsOrderManageQueuesAsync to run every 10 minutes to refresh the clients and sales channels
    RecurringJob.AddOrUpdate(
        "SaleChannelsOrderManageQueues", // Job name for refreshing clients and sale channels
        () => SaleChannelsOrderManageQueuesAsync(), // Main task to refresh clients and sale channels
        "*/10 * * * *" // Run every 10 minutes
    );
    await Task.CompletedTask; // Explicitly return a completed task if no async work is done in this method

  }
  public async Task SaleChannelsOrderManageQueuesAsync()
  {
    var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.AdminControlPanKey, (int)EnumEnvironmentType.Live);
    if (mcconfig is not null)
    {
      var result = await _sharedClientRepository.GetAllClients(mcconfig!.Value!);
      if (!string.IsNullOrEmpty(result))
      {
        var deserializedResponse = JsonConvert.DeserializeObject<AuthResponseModel<List<ClientSummaryResponseModel>>>(result);


        var saleChannelConfig = await _jobRepository.GetAllAutoSyncSaleChannelsForJob(deserializedResponse!.result!);
        //get all sale channels like, shopify/woocomerecec etc
        var groupedSaleLookups = saleChannelConfig.GroupBy(x => x.SaleChannelLookupId);
        // list of channel like shopify/woocomere
        foreach (var saleChannelLookupGroup in groupedSaleLookups)
        {
          // run job for each channel like for shopify,woocomere and any other channel each new channel have its own job wich will rerun after 10 minut
          //ProcessSaleChannelOrder(saleChannelLookupGroup);
          var saleChannelLookupId = saleChannelLookupGroup.Key;

          var targetLookup = saleChannelConfig.FirstOrDefault(x => x.SaleChannelLookupId == saleChannelLookupId);
          if (targetLookup is not null)
          {
            #region main code
            //get sale channel by lookup like shopify/woocomerece
            var saleChannelService = _saleChannelFactory.GetSaleChannelService(saleChannelLookupId.GetValueOrDefault());
            if (saleChannelService is not null)
            {
              //filter all data for looped channel
              var filteredOrderData = saleChannelConfig.Where(x => x.SaleChannelLookupId == saleChannelLookupId);
              //make que for each sale channel

              await saleChannelService.ProcessSaleChannelOrdersAsync(filteredOrderData);


              var jobName = $"{saleChannelService.GetType().Name}_OrderProcessing";
              //// Schedule a recurring job for the current sale channel every 10 minutes
              var jobid = BackgroundJob.Enqueue(
                    jobName, // Job name based on the sale channel
                    () => saleChannelService.ProcessSaleChannelOrdersAsync(filteredOrderData)
                );

              //  CheckJobState(jobName);
            }
            #endregion
          }
        }
      }
    }
  }
  public void CheckJobState(string jobId)
  {
    var monitoringApi = JobStorage.Current.GetConnection();
    var jobDetails = (JobStorageConnection)monitoringApi;

    if (jobDetails != null)
    {
      var jobData = jobDetails.GetJobData(jobId);
      if (jobData != null)
      {
        var jobState = jobData.State;
        Console.WriteLine($"Job {jobId} is in {jobState} state.");
      }
      else
      {
        Console.WriteLine($"No job found with ID: {jobId}");
      }
    }
    else
    {
      Console.WriteLine("Failed to get job storage connection.");
    }
  }
}
