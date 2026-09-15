using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetAirWayBillWithDynamicTemplate;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetTrackingHistory;
using Shipra.Backend.API.Application.Features.ReturnFeatures.Commands.CreateReturnOrder;
using Shipra.Backend.API.Application.Features.ReturnFeatures.Query.GetAllClientReturnReasonForSelection;
using Shipra.Backend.API.Application.Features.ReturnFeatures.Query.GetAllRefundTypeLookupForSelection;
using Shipra.Backend.API.Application.Features.ReturnFeatures.Query.GetAllReturnStatusLookupForSelection;
using Shipra.Backend.API.Application.Features.UserRoleAndPermissionFeature.Query.CheckAllPermissionActionExist;
using Shipra.Backend.API.Application.Helpers.Reporting;

namespace Shipra.Backend.API.Web.Api.OpenApis;

public class TrackingController : BaseApiController
{
  public TrackingController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }
  [HttpPost("GetTrackingHistory")]
  public async Task<ActionResult> GetTrackingHistory([FromBody] GetTrackingHistoryQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("CreateReturnOrder")]
  public async Task<ActionResult> CreateReturnOrder([FromForm] CreateReturnOrderCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetAllRefundTypeForSelection")]
  public async Task<ActionResult> GetAllRefundTypeLookupForSelection([FromQuery] GetAllRefundTypeLookupForSelectionQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetAllClientReturnReasonForSelection")]
  public async Task<ActionResult> GetAllClientReturnReasonForSelection([FromQuery] GetAllClientReturnReasonForSelectionQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetAllReturnStatusForSelection")]
  public async Task<ActionResult> GetAllReturnStatusForSelection([FromQuery] GetAllReturnStatusLookupForSelection request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetWayBillsByOrderNo")]
  public async Task<ActionResult> GetWayBillsByOrderNos([FromBody] GetAirWayBillWithDynamicTemplateQuery request, CancellationToken cancellationToken = default)
  { 
      // download shipra awb 
      var result = await Mediator.Send(request, cancellationToken);

      var file = DirectoryHelper.GetRandomNameForPdf("Awb");
      try
      {
        return File(result.Result!, PDFDocumentGenerator.ContentType, file);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"An error occurred: {ex.Message}");
      }

      return Ok(result); 
  }
}
