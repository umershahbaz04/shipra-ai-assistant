using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.Common.Security;
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
using Shipra.Backend.API.Application.Features.ExampleFeatures.Commands;
using Shipra.Backend.API.Application.Features.OrderTypeLookupFeatures.Query.GetAllOrderTypeLookupQuery;
using Shipra.Backend.API.Application.Features.PaymentMethodLookupFeatures.Query.GetAllPaymentMethodLookupQuery;
using Shipra.Backend.API.Application.Features.StationLookupFeatures.Query.GetAllStationLookupQuery;
using Shipra.Backend.API.Application.Features.TotalProcessFeature.Query.GetAllPaymentLinkStatusLookup;
using Shipra.Backend.API.Application.Helpers.Reporting;

namespace Shipra.Backend.API.Web.Api;
[Authorize]
public class CommonLookupController : BaseApiController
{
  public CommonLookupController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }

  #region query

  [HttpGet("GenerateExceptionExample")]
  public async Task<ActionResult> GenerateExceptionExample(CancellationToken cancellationToken = default)
  {
    GenerateExceptionExampleCommand request = new GenerateExceptionExampleCommand();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetAllRegionTimeZone")]
  public async Task<ActionResult> GetAllRegionTimeZone(CancellationToken cancellationToken = default)
  {
    GetAllRegionTimeZoneQuery request = new GetAllRegionTimeZoneQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetAllONGFTypeLookup")]
  public async Task<ActionResult> GetAllONGFTypeLookup(CancellationToken cancellationToken = default)
  {
    GetAllONGFTypeLookupQuery request = new GetAllONGFTypeLookupQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetAllSCFolderLookupForSelection")]
  public async Task<ActionResult> GetAllSCFolderLookup(CancellationToken cancellationToken = default)
  {
    GetAllSCFolderLookupForSelectionQuery request = new GetAllSCFolderLookupForSelectionQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetAllCarrierTrackingStatusLookupForSelection")]
  public async Task<ActionResult> GetAllCarrierTrackingStatusLookupForSelection(CancellationToken cancellationToken = default)
  {
    GetAllClientCarrierTrackingStatusLookupQuery request = new GetAllClientCarrierTrackingStatusLookupQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetAllAddressTypeLookup")]
  public async Task<ActionResult> GetAllAddressTypeLookup(CancellationToken cancellationToken = default)
  {
    GetAllAddressTypeLookupQuery request = new GetAllAddressTypeLookupQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetAllExpenseCategoryLookup")]
  public async Task<ActionResult> GetAllExpenseCategoryLookup(CancellationToken cancellationToken = default)
  {
    GetAllExpenseCategoryLookupQuery request = new GetAllExpenseCategoryLookupQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetAllFullFillmentStatusLookup")]
  public async Task<ActionResult> GetAllFullFillmentStatusLookup(CancellationToken cancellationToken = default)
  {
    GetAllFullFillmentStatusLookupQuery request = new GetAllFullFillmentStatusLookupQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetAllLookupAdjustReason")]
  public async Task<ActionResult> GetAllLookupAdjustReason(CancellationToken cancellationToken = default)
  {
    GetAllLookupAdjustReasonQuery request = new GetAllLookupAdjustReasonQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  } 
  [HttpGet("GetClientInfo")]
  public async Task<ActionResult> GetClientCountry()
  {
    var request = new GetClientCountryQuery();
    var response = await Mediator.Send(request, default);
    return Ok(response);
  }
  [HttpGet("GetGoogleMapRestrictedCountry")]
  public async Task<ActionResult> GetGoogleMapRestrictedCountry()
  {
    var request = new GetGoogleMapRestrictedCountryQuery();
    var response = await Mediator.Send(request, default);
    return Ok(response);
  }
  [HttpGet("GetAllPaymentStatusLookup")]
  public async Task<ActionResult> GetAllPaymentStatusLookup(CancellationToken cancellationToken = default)
  {
    GetAllPaymentStatusLookupQuery request = new GetAllPaymentStatusLookupQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetAllProductOptionLookup")]
  public async Task<ActionResult> GetAllProductOptionLookup(CancellationToken cancellationToken = default)
  {
    GetAllProductOptionLookupQuery request = new GetAllProductOptionLookupQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetAllWhatsAppCategoryTypeLookup")]
  public async Task<ActionResult> GetAllWhatsAppCategoryTypeLookup(CancellationToken cancellationToken = default)
  {
    GetAllWhatsAppCategoryTypeQuery request = new GetAllWhatsAppCategoryTypeQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetAllOrderTypeLookup")]
  public async Task<ActionResult> GetAllOrderTypeLookup(CancellationToken cancellationToken = default)
  {
    GetAllOrderTypeLookupQuery request = new GetAllOrderTypeLookupQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetAllPaymentMethodLookup")]
  public async Task<ActionResult> GetAllPaymentMethodLookup(CancellationToken cancellationToken = default)
  {
    GetAllPaymentMethodLookupQuery request = new GetAllPaymentMethodLookupQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetAllStationLookup")]
  public async Task<ActionResult> GetAllStationLookup(CancellationToken cancellationToken = default)
  {
    GetAllStationLookupQuery request = new GetAllStationLookupQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetAllPaymentLinkStatusLookup")]
  public async Task<ActionResult> GetAllPaymentLinkStatusLookup(CancellationToken cancellationToken = default)
  {
    GetAllPaymentLinkStatusLookupQuery request = new GetAllPaymentLinkStatusLookupQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("DownloadApiPostManCollection")]
  public async Task<ActionResult> DownloadApiPostManCollection(CancellationToken cancellationToken = default)
  {
    var result = new ServiceResultDTO();
    GetApiPostManCollectionQuery request = new GetApiPostManCollectionQuery();
    result = await Mediator.Send(request, cancellationToken);
    var file = DirectoryHelper.GetRandomNameForPdf("collection");
    try
    {
      return File(result.Result!, "application/json", file);
    }
    catch (Exception ex)
    {
      Console.WriteLine($"An error occurred: {ex.Message}");
    }
    return Ok(result);
  }
  #endregion
}
