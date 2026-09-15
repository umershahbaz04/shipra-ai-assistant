using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.ClientFeatures.Query.GetApiPostManCollection;
using Shipra.Backend.API.Application.Features.CommonFeatures.Command.UploadShipraStaticContent;
using Shipra.Backend.API.Application.Features.CommonFeatures.Query.GetAllAddressTypeLookupQuery;
using Shipra.Backend.API.Application.Features.CommonFeatures.Query.GetAllCarrierTrackingStatusLookup;
using Shipra.Backend.API.Application.Features.CommonFeatures.Query.GetAllExpenseCategoryQuery;
using Shipra.Backend.API.Application.Features.CommonFeatures.Query.GetAllFullFillmentStatusLookupQuery;
using Shipra.Backend.API.Application.Features.CommonFeatures.Query.GetAllLookupAdjustReasonQuery;
using Shipra.Backend.API.Application.Features.CommonFeatures.Query.GetAllONGFTypeLookup;
using Shipra.Backend.API.Application.Features.CommonFeatures.Query.GetAllPaymentStatusLookupQuery;
using Shipra.Backend.API.Application.Features.CommonFeatures.Query.GetAllProductOptionLookupQuery;
using Shipra.Backend.API.Application.Features.CommonFeatures.Query.GetAllRegionTimeZone;
using Shipra.Backend.API.Application.Features.CommonFeatures.Query.GetAllSCFolderLookup;
using Shipra.Backend.API.Application.Features.CommonFeatures.Query.GetAllWhatsAppCategoryLookupQuery;
using Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetClientCountry;
using Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetGoogleMapRestrictedCountry;
using Shipra.Backend.API.Application.Features.OrderTypeLookupFeatures.Query.GetAllOrderTypeLookupQuery;
using Shipra.Backend.API.Application.Features.PaymentMethodLookupFeatures.Query.GetAllPaymentMethodLookupQuery;
using Shipra.Backend.API.Application.Features.StationLookupFeatures.Query.GetAllStationLookupQuery;
using Shipra.Backend.API.Application.Features.TotalProcessFeature.Query.GetAllPaymentLinkStatusLookup;

namespace Shipra.Backend.API.Web.Api.OpenApis;

public class CommonLookupController : BaseApiController
{
  public CommonLookupController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }
  #region command
  [HttpPost("UploadShipraStaticContent")]
  public async Task<ActionResult> UploadShipraStaticContent([FromForm] UploadShipraStaticContentCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion

  #region query

  [HttpGet("GetAllRegionTimeZoneAdmin")]
  public async Task<ActionResult> GetAllRegionTimeZone(CancellationToken cancellationToken = default)
  {
    GetAllRegionTimeZoneQuery request = new GetAllRegionTimeZoneQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
 
  [HttpGet("GetAllSCFolderLookupForSelectionAdmin")]
  public async Task<ActionResult> GetAllSCFolderLookup(CancellationToken cancellationToken = default)
  {
    GetAllSCFolderLookupForSelectionQuery request = new GetAllSCFolderLookupForSelectionQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  } 
  [HttpGet("GetAllFullFillmentStatusLookupAdmin")]
  public async Task<ActionResult> GetAllFullFillmentStatusLookup(CancellationToken cancellationToken = default)
  {
    GetAllFullFillmentStatusLookupQuery request = new GetAllFullFillmentStatusLookupQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }   
  [HttpGet("GetAllPaymentStatusLookupAdmin")]
  public async Task<ActionResult> GetAllPaymentStatusLookup(CancellationToken cancellationToken = default)
  {
    GetAllPaymentStatusLookupQuery request = new GetAllPaymentStatusLookupQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }  

  [HttpGet("GetAllOrderTypeLookupAdmin")]
  public async Task<ActionResult> GetAllOrderTypeLookup(CancellationToken cancellationToken = default)
  {
    GetAllOrderTypeLookupQuery request = new GetAllOrderTypeLookupQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetAllPaymentMethodLookupAdmin")]
  public async Task<ActionResult> GetAllPaymentMethodLookup(CancellationToken cancellationToken = default)
  {
    GetAllPaymentMethodLookupQuery request = new GetAllPaymentMethodLookupQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }  
  #endregion
}
