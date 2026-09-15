using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Common.CustomBinder;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrder;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.DeleteOrder;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.RefreshCarrierStatus;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.UpdateOrderAmount;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.UpdateOrderLatLng;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetAllOrders;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetAllOrderStatusReport;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetOrderCount;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.ValidatedOrderAddressForCarrier;
using Shipra.Backend.API.Application.Features.ProductFeatures.Query.GetAllImageGalleries;
using Shipra.Backend.API.Application.Features.ProductFeatures.Query.GetProductById;
using Shipra.Backend.API.Application.Features.ProductFeatures.Query.GetProductDetailByLinkToken;
using Shipra.Backend.API.Application.Features.ShipmentFeatures.Query.GetOrderInfoByOrderNoForPopUp;
using Shipra.Backend.API.Application.Features.StationLookupFeatures.Query.GetAllStationLookupQuery;
using Shipra.Backend.API.Application.Features.StoreFeatures.Query.GetAllStoresQuery;
using Shipra.Backend.API.Application.Features.StoreFeatures.Query.GetStoresFroSelection;

namespace Shipra.Backend.API.Web.Api.OpenApis;

public class OrderController : BaseApiController
{
  public OrderController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }
  [HttpPost("GetAllOrdersForAdmin")]
  public async Task<ActionResult> GetAllOrdersForAdmin([FromBody] GetAllOrdersQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetShipmentInfoByOrderNoAdmin")]
  public async Task<ActionResult> GetShipmentInfoByOrderNo([FromBody] GetOrderInfoByOrderNoForPopUpQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("RefreshCarrierStatusAdmin")]
  public async Task<ActionResult> RefreshCarrierStatusAdmin([FromBody] RefreshCarrierStatusCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetOrderCountAdmin")]
  public async Task<ActionResult> GetOrderCountAdmin(CancellationToken cancellationToken = default)
  {
    GetOrderCountQuery request = new();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }  
  
  [HttpGet("GetProductDetailByLinkToken")]
  public async Task<ActionResult> GetProductDetailByLinkToken([FromQuery] GetProductDetailByLinkTokenQuery request, CancellationToken cancellationToken = default)
  { 
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("CreateOrderFromListing")] // never remove this mehtod used in (ipick)
  public async Task<ActionResult> CreateOrder([ModelBinder(BinderType = typeof(CreateOrderClientSideBinder))] CreateOrderCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetAllOrderStatusReportadmin")]
  public async Task<ActionResult> GetAllOrderStatusReport([FromBody] GetAllOrderStatusReportQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("UpdateOrderAmountadmin")]
  public async Task<ActionResult> UpdateOrderAmount([FromBody] UpdateOrderAmountCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetAllStoreForAdmin")]
  public async Task<ActionResult> GetAllStoreForAdmin(CancellationToken cancellationToken = default)
  {
    GetStoresForSelectionQuery request = new();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetAllStationForAdmin")]
  public async Task<ActionResult> GetAllStationForAdmin(CancellationToken cancellationToken = default)
  {
    GetAllStationLookupQuery request = new();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("ValidateOrderAddressForCarrierAdmin")]
  public async Task<ActionResult> ValidateOrderAddressForCarrierAdmin([FromBody] ValidateOrderAddressForCarrierQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("DeleteOrdersIpick")]
  public async Task<ActionResult> DeleteOrders([FromBody] DeleteOrdersCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetProductById")]
  public async Task<ActionResult> GetProductById([FromQuery] GetProductByIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetAllImageGalleries")]
  public async Task<ActionResult> GetAllImageGalleries(CancellationToken cancellationToken = default)
  {
    GetAllImageGalleriesQuery request = new();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("UpdateOrderLatAndLng")]
  public async Task<ActionResult> UpdateOrderLatAndLng([FromBody] UpdateOrderLatLngCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

}
