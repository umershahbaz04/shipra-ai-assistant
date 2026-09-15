using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Common.Security;
using Shipra.Backend.API.Application.Features.DashboardFeatures.CarrierActivity.Query;
using Shipra.Backend.API.Application.Features.DashboardFeatures.Finance.Query.GetTotalCollected;
using Shipra.Backend.API.Application.Features.DashboardFeatures.Finance.Query.GetTotalPurchaseStockValue;
using Shipra.Backend.API.Application.Features.DashboardFeatures.Finance.Query.GetTotalStockValue;
using Shipra.Backend.API.Application.Features.DashboardFeatures.Finance.Query.GetTotalUncollected;
using Shipra.Backend.API.Application.Features.DashboardFeatures.Query.CarrierActivity.Query;
using Shipra.Backend.API.Application.Features.DashboardFeatures.Query.CarrierActivity.Query.GetCarrierStats;
using Shipra.Backend.API.Application.Features.DashboardFeatures.Query.GetAllItemCount;
using Shipra.Backend.API.Application.Features.DashboardFeatures.Query.GetAllItemsGroupCount;
using Shipra.Backend.API.Application.Features.DashboardFeatures.Query.GetAllOrderCountWithCarrier;
using Shipra.Backend.API.Application.Features.DashboardFeatures.Query.GetAllOrderCountWithCarrierAndCODAmount;
using Shipra.Backend.API.Application.Features.DashboardFeatures.Query.GetDeliveryRatioCount;
using Shipra.Backend.API.Application.Features.DashboardFeatures.Query.GetLowStockItemsCount;
using Shipra.Backend.API.Application.Features.DashboardFeatures.Query.GetProductCounts;
using Shipra.Backend.API.Application.Features.DashboardFeatures.Query.GetTopSellingItemsCount;
using Shipra.Backend.API.Application.Features.DashboardFeatures.Query.GetTotalActiveCarrierCount;
using Shipra.Backend.API.Application.Features.DashboardFeatures.Query.GetTotalCarriers;
using Shipra.Backend.API.Application.Features.DashboardFeatures.Query.GetTotalStoreCount;
using Shipra.Backend.API.Application.Features.DashboardFeatures.Query.OrderActivity;
using Shipra.Backend.API.Application.Features.DashboardFeatures.Query.GetSaleDashboardChannels;
using Shipra.Backend.API.Application.Features.DashboardFeatures.Query.GetSaleDashboardProducts;
using Shipra.Backend.API.Application.Features.DashboardFeatures.Query.GetSaleDashboardStores;
using Shipra.Backend.API.Web.Api;

[Authorize] 
public class DashboardController : BaseApiController
{
  public DashboardController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }
  #region query
  [HttpPost("GetTotalCarriers")]
  public async Task<ActionResult> DashboardGetTotalCarriers([FromBody] DashboardGetTotalCarriersQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetActiveCarriers")]
  public async Task<ActionResult> DashboardGetActiveCarriers([FromBody] GetTotalActiveCarrierCountQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetTotalStoreCount")]
  public async Task<ActionResult> GetTotalStoreCount([FromBody] GetTotalStoreCountQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetTotalUncollected")]
  public async Task<ActionResult> GetTotalUncollected([FromBody] GetTotalUncollectedQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetTotalCollected")]
  public async Task<ActionResult> GetTotalCollected([FromBody] GetTotalCollectedQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  } 
  [HttpPost("GetCarrierActivityWithDetail")]
  public async Task<ActionResult> GetCarrierActivityWithDetail([FromBody] GetCarrierActivityWithDetailQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  } 
  [HttpPost("GetCarrierStats")]
  public async Task<ActionResult> GetCarrierStats([FromBody] GetCarrierStatsQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetCarrierDashboardStats")]
  public async Task<ActionResult> GetCarrierDashboardStats([FromBody] GetCarrierDashboardStatsQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetTotalStockValue")]
  public async Task<ActionResult> GetTotalStockValue([FromBody] GetTotalStockValueQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }  
  [HttpPost("GetTotalPurchaseStockValue")]
  public async Task<ActionResult> GetTotalPurchaseStockValue([FromBody] GetTotalPurchaseStockValueQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetProductCounts")]
  public async Task<ActionResult> DashboardGetProductCounts([FromBody] DashboardGetProductCountsQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetTotalNoofOrdersPlacedCounts")]
  public async Task<ActionResult> DashboardGetTotalNoofOrdersPlaced([FromBody] DashboardGetTotalNoofOrdersPlacedQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);

  }
  [HttpPost("GetAllItemCount")]
  public async Task<ActionResult> GetAllItemCount([FromBody] GetAllItemCountQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetAllProductVariantsCount")]
  public async Task<ActionResult> GetAllProductVariantsCount([FromBody] GetAllProductVariantsCountQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetLowStockItemsCount")]
  public async Task<ActionResult> GetLowStockItemsCount([FromBody] GetLowStockItemsCountQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("GetToBePackedCount")]
  public async Task<ActionResult> GetToBePackedCount([FromBody] GetToBePackedCountQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetToBeShippedCount")]
  public async Task<ActionResult> GetToBeShippedCount([FromBody] GetToBeShippedCountQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetTopSellingItemsCount")]
  public async Task<ActionResult> GetTopSellingItemsCount([FromBody] GetTopSellingItemsCountQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetDeliveryRatioCount")]
  public async Task<ActionResult> GetDeliveryRatioCount([FromBody] GetDeliveryRatioCountQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("GetTotalCompletedOrderCountWithCarrier")]
  public async Task<ActionResult> GetTotalCompletedOrderCountWithCarrier([FromBody] GetTotalCompletedOrderCountWithCarrierQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetTotalInprogressOrderCountWithCarrier")]
  public async Task<ActionResult> GetTotalInprogressOrderCountWithCarrier([FromBody] GetTotalInprogressOrderCountWithCarrierQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetReturnedOrderCount")]
  public async Task<ActionResult> GetReturnedOrderCount([FromBody] GetReturnedOrderCountQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetInProgressOrderCount")]
  public async Task<ActionResult> GetInProgressOrderCount([FromBody] GetInProgressOrderCountQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetRegularOrderCount")]
  public async Task<ActionResult> GetRegularOrderCount([FromBody] GetRegularOrderCountQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetFulfillableOrderCount")]
  public async Task<ActionResult> GetFulfillableOrderCount([FromBody] GetFulfillableOrderCountQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetDelieveredOrderCount")]
  public async Task<ActionResult> GetDelieveredOrderCount([FromBody] GetDeliveredOrderCountQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("GetAllOrderCountWithCarrier")]
  public async Task<ActionResult> GetAllOrderCountWithCarrier([FromBody] GetAllOrderCountWithCarrier request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("GetAllOrderCountWithCarrierAndCODAmount")]
  public async Task<ActionResult> GetAllOrderCountWithCarrierAndCODAmount([FromBody] GetAllOrderCountWithCarrierAndCODAmountQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("GetSaleDashboardChannels")]
  public async Task<ActionResult> GetSaleDashboardChannels([FromBody] GetSaleDashboardChannelsQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("GetSaleDashboardProducts")]
  public async Task<ActionResult> GetSaleDashboardProducts([FromBody] GetSaleDashboardProductsQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("GetSaleDashboardStores")]
  public async Task<ActionResult> GetSaleDashboardStores([FromBody] GetSaleDashboardStoresQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion
}
