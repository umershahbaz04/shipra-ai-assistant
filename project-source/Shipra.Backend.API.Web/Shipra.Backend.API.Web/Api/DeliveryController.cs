using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.Common.Security;
using Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetOrderAddressLAtLngByDeliveryNoteId;
using Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Command.CompleteDeliveryNote;
using Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Command.CreateDeliveryNoteDetailPendingForReturnStatus;
using Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Command.CreateMyCarrierReturnReport;
using Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Command.DeleteMyCarrierRetunReport;
using Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Command.RevertDeliveryNote;
using Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Command.DeleteDeliveryNoteDetailByOrderId;
using Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Command.UpdateDeliveryNoteInOperation;
using Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Command.UpdateOrderStatusOnDebrief;
using Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Command.DeleteDeliveryNote;
using Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Command.MergeDeliveryNotes;
using Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetAllDeliveryNote;
using Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetAllMyCarrierReturnReports;
using Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetDeliveryNoteById;
using Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetDeliveryNoteByNo;
using Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetDeliveryNoteDetailForDebrief;
using Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetCompletedDeliveryNoteExpenses;
using Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetDeliveryNotePaymentInfo;
using Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetMyCarrierShipmentsByReturnReportNo;
using Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetPDFMyCarrierReturnReport;
using Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetPDFRunSheet;
using Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Command.BatchOutScanDeliveryTask;
using Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Command.CreateDeliveryTask;
using Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Command.DeleteDeliveryTask;
using Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Command.RevertDeliveryTask;
using Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Command.UpdateDeliveryTask;
using Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Query.ExcelExportAllDeliveryTasks;
using Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Query.ExcelExportCODCollectionPendingsMyCarrier;
using Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Query.ExcelExportCODCollectionsMyCarrier;
using Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Query.GetAllCODCollectionPendingsMyCarrier;
using Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Query.GetAllCODCollectionsMyCarrier;
using Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Query.GetAllDeliveryTask;
using Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Query.GetAllDeliveryTaskStatusForSelection;
using Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Query.GetAllPendingForReturnShipment;
using Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Query.GetDeliveryTaskById;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Application.Helpers.Reporting;
using Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetDriverOrdersByDriverId;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetMapKey;

namespace Shipra.Backend.API.Web.Api;
[Authorize]
public class DeliveryController : BaseApiController
{
  public DeliveryController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }
  #region deliverytask
  [HttpPost("CreateDeliveryTask")]
  public async Task<ActionResult> CreateDeliveryTask([FromBody] CreateDeliveryTaskCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("CreateDeliveryTaskAndAddToExistingNote")]
  public async Task<ActionResult> CreateDeliveryTaskAndAddToExistingNote([FromBody] CreateDeliveryTaskAndAddToExistingNoteCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("UpdateDeliveryTask")]
  public async Task<ActionResult> UpdateDeliveryTask([FromBody] UpdateDeliveryTaskCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("UnAssignDeliveryTask")]
  public async Task<ActionResult> UnAssignDeliveryTask([FromBody] UnAssignDeliveryTaskByIdCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetAllDeliveryTask")]
  public async Task<ActionResult> GetAllDeliveryTask([FromBody] GetAllDeliveryTaskQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("ExcelExportDeliveryTasks")]
  public async Task<ActionResult> ExcelExportDeliveryTasks([FromBody] ExcelExportAllDeliveryTasksQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return File(response!.Result?.Bytes!, ExcelExportHelper.ExcelContentType, ExcelExportHelper.GetExcelFileName("DeliveryTasks"));
  }

  [HttpGet("GetDeliveryTaskById")]
  public async Task<ActionResult> GetDeliveryTaskById([FromQuery] GetDeliveryTaskByIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("RevertDeliveryTaskByOrderNos")]
  public async Task<ActionResult> RevertDeliveryTaskById([FromBody] RevertDeliveryTaskByOrderNosCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }


  #region Whatsapp Button Hnadler
  [HttpPost("SendLocationMessage")]
  public async Task<ActionResult> SendLocationMessage([FromBody] SendSmsCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion


  #endregion

  #region deliverynote
  [HttpPost("CreateDeliveryNoteDetailPendingForReturnStatus")]
  public async Task<ActionResult> CreateDeliveryNoteDetailPendingForReturnStatus([FromBody] CreateDeliveryNoteDetailPendingForReturnStatusCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("CreateMyCarrierReturnReport")]
  public async Task<ActionResult> CreateMyCarrierReturnReport([FromBody] CreateMyCarrierReturnReportCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("DeleteMyCarrierReturnReport")]
  public async Task<ActionResult> DeleteMyCarrierReturnReport([FromBody] DeleteMyCarrierReturnReportCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetAllMyCarrierReturnReports")]
  public async Task<ActionResult> GetAllMyCarrierReturnReports([FromBody] GetAllMyCarrierReturnReportsQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetMyCarrierShipmentsByReturnReportId")]
  public async Task<ActionResult> GetMyCarrierShipmentsByReturnReportId([FromQuery] GetMyCarrierShipmentsByReturnReportIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetPDFMyCarrierReturnReport")]
  public async Task<ActionResult> GetPDFMyCarrierReturnReport([FromBody] GetPDFMyCarrierReturnReportQuery request, CancellationToken cancellationToken = default)
  {
    var result = await Mediator.Send(request, cancellationToken);
    var file = DirectoryHelper.GetRandomNameForPdf("MyCarrierReturnReport");
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
  [HttpPost("UpdateDeliveryNoteInOperation")]
  public async Task<ActionResult> UpdateDeliveryNoteInOperation([FromBody] UpdateDeliveryNoteInOperationCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("GetAllDeliveryNote")]
  public async Task<ActionResult> GetAllDeliveryNote([FromBody] GetAllDeliveryNoteQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("DeleteDeliveryNoteById")]
  public async Task<ActionResult> DeleteDeliveryNoteById([FromBody] DeleteDeliveryNoteByIdCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("MergeDeliveryNotes")]
  public async Task<ActionResult> MergeDeliveryNotes([FromBody] MergeDeliveryNotesCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetOrderAddressLatLngByDeliveryNoteId")]
  public async Task<ActionResult> GetOrderAddressLAtLngByDeliveryNoteId([FromQuery] GetOrderAddressLAtLngByDeliveryNoteIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetDeliveryNoteById")]
  public async Task<ActionResult> GetDeliveryNoteById([FromQuery] GetDeliveryNoteByIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetDeliveryNoteByNo")]
  public async Task<ActionResult> GetDeliveryNoteByNo([FromBody] GetDeliveryNoteByNoQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetDeliveryNoteDetailForDebrief")]
  public async Task<ActionResult> GetDeliveryNoteDetailForDebrief([FromBody] GetDeliveryNoteDetailForDebriefQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetCompletedDeliveryNoteExpenses")]
  public async Task<ActionResult> GetCompletedDeliveryNoteExpenses([FromBody] GetCompletedDeliveryNoteExpensesQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetDriverOrdersByDriverId")]
  public async Task<ActionResult> GetDriverOrdersByDriverId([FromBody] GetDriverOrdersByDriverIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("CompleteDeliveryNote")]
  public async Task<ActionResult> CompleteDeliveryNote([FromBody] CompleteDeliveryNoteCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("RevertDeliveryNoteDetailByOrderId")]
  public async Task<ActionResult> RevertDeliveryNoteDetailByOrderId([FromBody] RevertDeliveryNoteDetailByOrderIdCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("UncompleteDeliveryNote")]
  public async Task<ActionResult> UncompleteDeliveryNote([FromBody] Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Command.UncompleteDeliveryNote.UncompleteDeliveryNoteCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("DeleteDeliveryNoteDetailByOrderId")]
  public async Task<ActionResult> DeleteDeliveryNoteDetailByOrderId([FromBody] DeleteDeliveryNoteDetailByOrderIdCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetDeliveryNotePaymentInfo")]
  public async Task<ActionResult> GetDeliveryNotePaymentInfo([FromQuery] GetDeliveryNotePaymentInfoQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetDriverLatestDeliveryNoteToday/{driverId}")]
  public async Task<ActionResult> GetDriverLatestDeliveryNoteToday([FromRoute] string driverId, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(new Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetDriverLatestDeliveryNoteToday.GetDriverLatestDeliveryNoteTodayQuery { DriverId = driverId }, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetPDFDeliveryRunSheet")]
  public async Task<ActionResult> GetPDFDeliveryRunSheet([FromQuery] GetPDFDeliveryRunSheetQuery request, CancellationToken cancellationToken = default)
  {
    var result = await Mediator.Send(request, cancellationToken);
    var file = DirectoryHelper.GetRandomNameForPdf("RunSheet");
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
  [HttpPost("GetAllPendingForReturnShipments")]
  public async Task<ActionResult> GetAllPendingForReturnShipments([FromBody] GetAllPendingForReturnShipmentQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetAllDeliveryTaskStatusForSelection")]
  public async Task<ActionResult> GetAllDeliveryTaskStatusForSelection(CancellationToken cancellationToken = default)
  {
    GetAllDeliveryTaskStatusForSelectionQuery request = new GetAllDeliveryTaskStatusForSelectionQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("ExcelExportCODCollectionPendingsMyCarrier")]
  public async Task<ActionResult> ExcelExportCODCollectionPendingsMyCarrier([FromBody] ExcelExportCODCollectionPendingsMyCarrierQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return File(response!.Result?.Bytes!, ExcelExportHelper.ExcelContentType, ExcelExportHelper.GetExcelFileName("CODCollectionPendings"));
  }
  [HttpPost("GetAllCODCollectionPendingsMyCarrier")]
  public async Task<ActionResult> GetAllCODCollectionPendingsMyCarrier([FromBody] GetAllCODCollectionPendingsMyCarrierQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("ExcelExportCODCollectionsMyCarrierQuery")]
  public async Task<ActionResult> ExcelExportCODCollectionsMyCarrierQuery([FromBody] ExcelExportCODCollectionsMyCarrierQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return File(response!.Result?.Bytes!, ExcelExportHelper.ExcelContentType, ExcelExportHelper.GetExcelFileName("CODCollections"));
  }
  [HttpPost("GetAllCODCollectionsMyCarrier")]
  public async Task<ActionResult> GetAllCODCollectionsMyCarrier([FromBody] GetAllCODCollectionsMyCarrierQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("UpdateOrderStatusOnDebrief")]
  public async Task<ActionResult> UpdateOrderStatusOnDebrief([FromBody] UpdateOrderStatusForCompleteOnDebriefCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  } 
  #endregion
}

