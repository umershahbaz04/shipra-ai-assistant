using Ardalis.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.CustomBinder;
using Shipra.Backend.API.Application.Common.Enums;
using Shipra.Backend.API.Application.Common.Filters;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.Common.Security;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Application.Features.CarrierFeatures.Commands.AssignCarrierManualAdmin;
using Shipra.Backend.API.Application.Features.CarrierFeatures.Query.GetAwbForCarrierByOrderNos;
using Shipra.Backend.API.Application.Features.CarrierFeatures.Query.GetAwbForCarrierByRefNos;
using Shipra.Backend.API.Application.Features.ClientOrderLabelFeature.Command.CreateClientOrderLabel;
using Shipra.Backend.API.Application.Features.ClientOrderLabelFeature.Command.CreateClientOrderLabelLookup;
using Shipra.Backend.API.Application.Features.ClientOrderLabelFeature.Command.DeleteClientOrderLabel;
using Shipra.Backend.API.Application.Features.ClientOrderLabelFeature.Command.DeleteClientOrderLabelLookup;
using Shipra.Backend.API.Application.Features.ClientOrderLabelFeature.Command.UpdateClientOrderLabelLookup;
using Shipra.Backend.API.Application.Features.ClientOrderLabelFeature.Query.GetAllClientOrderLabelLookup;
using Shipra.Backend.API.Application.Features.ClientOrderLabelFeature.Query.GetAllClientOrderLabelLookupForSelection;
using Shipra.Backend.API.Application.Features.ClientOrderLabelFeature.Query.GetClientOrderLabelLookupId;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.AssignStationToOrders;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.BatchOrderFulfillment;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.BatchUpdateOrderStatus;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateInvoiceByOrderNumber;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrder;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrderForShopify;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrderFromSaleChannelOrders;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrderUnMapped;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.DeleteOrder;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.DeleteOrderItems;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.GetProductNameByOrderDraftId;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.IntegrationToCarrier;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.OrdersArchive;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.RefreshCarrierStatus;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.RestoreOrderArchive;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.UpdateCustomerEmailAddress;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.UpdateOrder;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.UpdateOrderAmount;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.UpdateOrderDraft;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.UpdateOrderLatLng;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.UpdateOrderPaymentStatus;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.UpdateOrderWithSaleChannel;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.UpdateValidatedOrderForCarrier;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.UploadFileFullfilable;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.UploadFileRegular;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.UploadPaperlessDocuments;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.CheckIsDefaultCarrierOrders;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.ExcelExportAllOrders;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.ExcelExportOrdersArchive;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GenerateManifest;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetAirWayBillWithDynamicTemplate;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetAllArchiveOrders;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetAllOrderDraft;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetAllOrderPaymentLinks;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetAllOrders;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetAllOrdersByTenantId;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetAllOrdersDraftCount;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetAllOrdersForGeneratePaymentLink;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetAllOrdersForSalePerson;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetAllOrderStatusReport;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetCalculatedRateByCarrier;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetFullfilableOrderFile;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetMapApiKey;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetMapKey;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetOrderById;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetOrderDraftByDraftId;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetOrderInvoiceByOrderNo;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetOrderItemByOrderNo;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.CheckMobileNoDuplicate;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetOrderLastTrackingStatus;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetValidatedOrderAddressByActiveCarrier;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetWayBill4X6ByOrderNo;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.ValidatedOrderAddressForCarrier;
using Shipra.Backend.API.Application.Features.OrderNoteFeatures.Command.CreateOrderNote;
using Shipra.Backend.API.Application.Features.OrderNoteFeatures.Command.DeleteOrderNote;
using Shipra.Backend.API.Application.Features.OrderNoteFeatures.Query.GetOrderNoteById;
using Shipra.Backend.API.Application.Features.OrderTrackingHistoryFeatures.Command.CreateOrderTrackingHistory;
using Shipra.Backend.API.Application.Features.OrderTrackingHistoryFeatures.Query.GetOrderTrackingHistoryByOrderId;
using Shipra.Backend.API.Application.Features.ReturnFeatures.Commands.CreateReturnOrder;
using Shipra.Backend.API.Application.Features.ReturnFeatures.Query.GetAllClientReturnReasonForSelection;
using Shipra.Backend.API.Application.Features.TotalProcessFeature.Command.GeneratePaymentLink;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Application.Helpers.Reporting;

namespace Shipra.Backend.API.Web.Api;
[Authorize]
public class OrderController : BaseApiController
{
  private readonly IWebHostEnvironment _webHostEnvironment;

  public OrderController(IServiceProvider serviceProvider, IWebHostEnvironment webHostEnvironment) : base(serviceProvider)
  {
    this._webHostEnvironment = webHostEnvironment;
  }


  #region Upload File
  [HttpPost("GetOrderFileByOrderTypeId")]
  public async Task<ActionResult> GetRegularOrderFile([FromQuery] GetOrderFileByOrderTypeIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("UploadFileRegular")]
  public async Task<ActionResult> UploadFileRegular([FromForm] UploadFileRegularCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #region fullfilable
  [HttpPost("UploadFileFullfilable")]
  public async Task<ActionResult> UploadFileFullfilable([FromForm] UploadFileFullfilableCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion
  #endregion

  #region command
  [HttpPost("CreateOrder")]
  public async Task<ActionResult> CreateOrder([ModelBinder(BinderType = typeof(CreateOrderClientSideBinder))] CreateOrderCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }


  [HttpPost("CreateOrderForShopify")]
  public async Task<ActionResult> CreateOrderForShopify([FromBody] CreateOrderForShopifyCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("CreateOrderWithInvoice")]
  public async Task<ActionResult> CreateOrderWithInvoice([ModelBinder(BinderType = typeof(CreateOrderClientSideBinder))] CreateOrderCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    if (response.IsSuccess)
    {
      var result = response.Result;
      var createOrderResponse = (IEnumerable<dynamic>)result?.Data!;
      foreach (var obj in createOrderResponse)
      {
        CreateInvoiceByOrderNumberCommand invoicCommand = new CreateInvoiceByOrderNumberCommand();
        invoicCommand.OrderNo = obj.OrderNo;
        response = await Mediator.Send(invoicCommand, cancellationToken);
      }
    }
    return Ok(response);
  }
  [HttpPost("CreateOrderNote")]
  public async Task<ActionResult> CreateOrderNote([FromBody] CreateOrderNoteCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("CreateOrderTrackingHistoryComments")]
  public async Task<ActionResult> CreateOrderTrackingHistoryComments([FromBody] CreateOrderTrackingHistoryCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("DeleteOrderNoteById")]
  public async Task<ActionResult> DeleteOrderNoteById([FromBody] DeleteOrderNoteCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("UpdateCustomerEmailAddress")]
  public async Task<ActionResult> UpdateCustomerEmailAddress([FromBody] UpdateCustomerEmailAddressCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("CheckMobileNoDuplicate")]
  public async Task<ActionResult> CheckMobileNoDuplicate(string mobileNo, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(new CheckMobileNoDuplicateQuery(mobileNo), cancellationToken);
    return Ok(response);
  }
  [HttpPost("UpdateOrder")]
  public async Task<ActionResult> UpdateOrder([ModelBinder(BinderType = typeof(UpdateOrderClientSideBinder))] UpdateOrderCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("UpdateOrderWithInvoice")]
  public async Task<ActionResult> UpdateOrderWithInvoice([FromBody] UpdateOrderCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    if (response.IsSuccess)
    {
      var result = response.Result;
      var obj = result?.Data!;
      if (obj != null)
      {
        CreateInvoiceByOrderNumberCommand invoicCommand = new CreateInvoiceByOrderNumberCommand();
        invoicCommand.OrderNo = obj!.OrderNo;
        response = await Mediator.Send(invoicCommand, cancellationToken);
      }
    }
    return Ok(response);
  }
  [HttpPost("UpdateOrderAmount")]
  public async Task<ActionResult> UpdateOrderAmount([FromBody] UpdateOrderAmountCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("UpdateValidatedOrderForCarrier")]
  public async Task<ActionResult> UpdateValidatedOrderForCarrier([FromBody] UpdateValidatedOrderAddressForCarrierQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("DeleteOrderItemById")]
  public async Task<ActionResult> DeleteOrderItemById([FromBody] DeleteOrderItemByIdCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("DeleteOrders")]
  public async Task<ActionResult> DeleteOrders([FromBody] DeleteOrdersCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("BatchUpdateOrderStatus")]
  public async Task<ActionResult> BatchUpdateOrderStatus([FromBody] BatchUpdateOrderStatusCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("UpdateOrderWithSaleChannel")]
  public async Task<ActionResult> UpdateOrderWithSaleChannel([FromBody] UpdateOrderWithSaleChannelCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("BatchOrderFulfillment")]
  public async Task<ActionResult> BatchOrderFulfillment([FromBody] BatchOrderFulfillmentCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("AssignStationToOrders")]
  public async Task<ActionResult> AssignStationToOrders([FromBody] AssignStationToOrdersCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("UpdateOrderPaymentStatus")]
  public async Task<ActionResult> UpdateOrderPaymentStatus([FromBody] UpdateOrderPaymentStatusCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("UpdateOrderLatLng")]
  public async Task<ActionResult> UpdateOrderLatLng([FromBody] UpdateOrderLatLngCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("CreateInvoiceByOrderNumber")]
  public async Task<ActionResult> CreateInvoiceByOrderNumber([FromBody] CreateInvoiceByOrderNumberCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("GenerateTotalProcessPaymentLink")]
  public async Task<ActionResult> GeneratePaymentLink([FromBody] GenerateTotalProcessPaymentLinkCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("DeleteOrderDraft")]
  public async Task<ActionResult> DeleteOrderDraft([FromBody] DeleteOrderDraftCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion
  [HttpPost("GeneretePDF")]
  public ActionResult GeneretePDF(IFormFile uploadedFile)
  {
    byte[] test = new byte[1];
    string? uniqueFileName = null;
    if (uploadedFile.ContentType != null)
    {
      string fileUploadsFolder = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, "HtmlSample");
      if (!Directory.Exists(fileUploadsFolder))
      { //check if the folder exists;
        Directory.CreateDirectory(fileUploadsFolder);
      }
      uniqueFileName = Guid.NewGuid().ToString() + "_" + uploadedFile.FileName;
      string fileSourcePath = System.IO.Path.Combine(fileUploadsFolder, uniqueFileName);

      using (var fileStream = new FileStream(fileSourcePath, FileMode.Create))
      {
        uploadedFile.CopyTo(fileStream);
      }

      if (System.IO.File.Exists(fileSourcePath))
      {
        // Read entire text file content in one string
        string htmlSourceString = System.IO.File.ReadAllText(fileSourcePath);
        var stream = PDFDocumentGenerator.GeneratePDF(htmlSourceString);
        return File(stream.ToArray(), "application/pdf", Guid.NewGuid().ToString() + ".pdf");
      }
    }
    return File(test, "application/pdf", "Grid.pdf");
  }

  #region query
  [HttpGet("GetOrderById")]
  public async Task<ActionResult> GetOrderById([FromQuery] GetOrderByIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetAllOrders")]
  public async Task<ActionResult> GetAllOrders([FromBody] GetAllOrdersQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetAllOrdersForGeneratePaymentLink")]
  public async Task<ActionResult> GetAllOrdersForGeneratePaymentLink([FromBody] GetAllOrdersForGeneratePaymentLinkQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetAllOrderPaymentLinks")]
  public async Task<ActionResult> GetAllOrderPaymentLinks([FromBody] GetAllOrderPaymentLinksQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetAllOrdersForSalePerson")]
  public async Task<ActionResult> GetAllOrdersForSalePerson([FromBody] GetAllOrdersForSalePersonQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetOrderItemByOrderNo")]
  public async Task<ActionResult> GetOrderItemByOrderNo([FromQuery] GetOrderItemByOrderNoQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("ExcelExportOrders")]
  public async Task<ActionResult> ExcelExportOrders([FromBody] ExcelExportAllOrdersQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return File(response!.Result?.Bytes!, ExcelExportHelper.ExcelContentType, ExcelExportHelper.GetExcelFileName("Orders"));

  }
  [HttpGet("GetOrderTrackingHistoryByOrderNo")]
  public async Task<ActionResult> GetOrderTrackingHistoryByOrderNo([FromQuery] GetOrderTrackingHistoryByOrderNoQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetAllOrdersByTenantId")]
  public async Task<ActionResult> GetAllOrdersByTenantId([FromBody] GetAllOrdersByTenantIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetOrderInvoiceByOrderNos")]
  public async Task<ActionResult> GetOrderInvoiceByOrderNo([FromBody] GetOrderInvoiceByOrderNoQuery request, CancellationToken cancellationToken = default)
  {
    var result = await Mediator.Send(request, cancellationToken);
    var file = DirectoryHelper.GetRandomNameForPdf("Invoice");
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
  //[HttpPost("GetWayBillsByOrderNos")]
  //public async Task<ActionResult> GetWayBillsByOrderNos([FromBody] GetAirWayBillsRequestModel model, CancellationToken cancellationToken = default)
  //{
  //  if (ModelState.IsValid)
  //  {
  //    var result = new ServiceResultDTO();
  //    // download shipra awb
  //    GetWayBill4X6ByOrderNosQuery request = new GetWayBill4X6ByOrderNosQuery();
  //    request.OrderNos = model.OrderNos;
  //    result = await Mediator.Send(request, cancellationToken);

  //    var file = DirectoryHelper.GetRandomNameForPdf("Awb");
  //    try
  //    {
  //      return File(result.Result!, PDFDocumentGenerator.ContentType, file);
  //    }
  //    catch (Exception ex)
  //    {
  //      Console.WriteLine($"An error occurred: {ex.Message}");
  //    }

  //    return Ok(result);
  //  }
  //  else
  //  {
  //    return BadRequest();
  //  }
  //} 
  [HttpPost("GetWayBillsByOrderNos")]
  public async Task<ActionResult> GetWayBillsByOrderNos([FromBody] GetAirWayBillWithDynamicTemplateQuery request, CancellationToken cancellationToken = default)
  {
    if (ModelState.IsValid)
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
    else
    {
      return BadRequest();
    }
  }
  [HttpPost("GetWayBillsByOrderNosWithDynamicTemplate")]
  public async Task<ActionResult> GetAirWayBillWithDynamicTemplate([FromBody] GetAirWayBillWithDynamicTemplateQuery request, CancellationToken cancellationToken = default)
  {
    if (ModelState.IsValid)
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
    else
    {
      return BadRequest();
    }
  }
  [HttpPost("GetCarrierWayBillsByOrderNos")]
  public async Task<ActionResult> GetCarrierWayBillsByOrderNos([FromBody] GetAirWayBillsRequestModel model, CancellationToken cancellationToken = default)
  {
    if (ModelState.IsValid)
    {
      var result = new ServiceResultDTO();

      //CheckIsDefaultCarrierOrdersQuery checkIsDefaultCarrierOrders = new CheckIsDefaultCarrierOrdersQuery();
      //checkIsDefaultCarrierOrders.OrderNos = model.OrderNos;
      //result = await Mediator.Send(checkIsDefaultCarrierOrders, cancellationToken);

      //if (result.IsSuccess)
      //{
      //var isClientCarrier = result.Result!.IsClientCarrier;
      //if (isClientCarrier == true)
      //{
      //  // download shipra awb
      //  GetWayBill4X6ByOrderNosQuery request = new GetWayBill4X6ByOrderNosQuery();
      //  request.OrderNos = model.OrderNos;
      //  result = await Mediator.Send(request, cancellationToken);
      //}
      //else
      //{
      //get awb from carrier and download
      GetAwbForCarrierByOrderNosQuery request = new GetAwbForCarrierByOrderNosQuery();
      request.orderNos = model.OrderNos;
      result = await Mediator.Send(request, cancellationToken);
      //}
      var file = DirectoryHelper.GetRandomNameForPdf("Awb");
      try
      {
        return File(result.Result!, PDFDocumentGenerator.ContentType, file);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"An error occurred: {ex.Message}");
      }
      //}

      return Ok(result);
    }
    else
    {
      return BadRequest();
    }
  }
  [HttpPost("GetAwbForCarrierByRefNos")]
  public async Task<ActionResult> GetAwbForCarrierByRefNos([FromBody] GetAwbForCarrierByRefNosQuery request, CancellationToken cancellationToken = default)
  {
    var result = await Mediator.Send(request, cancellationToken);
    //}
    var file = DirectoryHelper.GetRandomNameForPdf("Awb");
    try
    {
      return File(result.Result!, PDFDocumentGenerator.ContentType, file);
    }
    catch (Exception ex)
    {
      Console.WriteLine($"An error occurred: {ex.Message}");
    }
    //}

    return Ok(result);

  }

  [HttpGet("GetShippingLabelByOrderNos")]

  public async Task<ActionResult> GetShippingLabelByOrderNo([FromQuery] GetWayBill4X6ByOrderNosQuery request, CancellationToken cancellationToken = default)
  {
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
  [HttpGet("GetAirwayBillTypes")]
  public ActionResult GetAirwayBillTypes()
  {
    var response = GetAirwaysBill.GetList().OrderBy(x => x.DisplayOrder);
    return Ok(response);
  }
  [HttpGet("GetAllOrderNotesByOrderNo")]
  public async Task<ActionResult> GetAllOrderNotesByOrderNo([FromQuery] GetOrderNoteByOrderNoQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetAllOrderTrackingHistoryByOrderId")]
  public async Task<ActionResult> GetAllOrderTrackingHistoryByOrderId([FromQuery] GetOrderTrackingHistoryByOrderNoQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetOrderLastTrackingStatus")]
  public async Task<ActionResult> GetOrderLastTrackingStatus([FromQuery] GetOrderLastTrackingStatusQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #region order labels
  [HttpPost("CreateClientOrderLabel")]
  public async Task<ActionResult> CreateClientOrderLabel([FromBody] CreateClientOrderLabelCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("CreateClientOrderLabelLookup")]
  public async Task<ActionResult> CreateClientOrderLabelLookup([FromBody] CreateClientOrderLabelLookupCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("DeleteClientOrderLabel")]
  public async Task<ActionResult> DeleteClientOrderLabel([FromBody] DeleteClientOrderLabelCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("DeleteClientOrderLabelLookup")]
  public async Task<ActionResult> DeleteClientOrderLabelLookup([FromBody] DeleteClientOrderLabelLookupCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("UpdateClientOrderLabelLookup")]
  public async Task<ActionResult> UpdateClientOrderLabelLookup([FromBody] UpdateClientOrderLabelLookupCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetAllClientOrderLabelLookup")]
  public async Task<ActionResult> GetAllClientOrderLabelLookup([FromBody] GetAllClientOrderLabelLookupQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetClientOrderLabelLookupId")]
  public async Task<ActionResult> GetClientOrderLabelLookupId([FromBody] GetClientOrderLabelLookupIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetAllClientOrderLabelLookupForSelection")]
  public async Task<ActionResult> GetAllClientOrderLabelLookupForSelection(CancellationToken cancellationToken = default)
  {
    GetAllClientOrderLabelLookupForSelectionQuery request = new();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion
  #endregion

  #region integration carrier
  [HttpPost("AssignToCarrier")]
  public async Task<ActionResult> IntegrationToCarrier([FromBody] AssignToCarrierCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetCalculatedRateByCarrier")]
  public async Task<ActionResult> GetCalculatedRateByCarrier([FromBody] GetCalculatedRateByCarrierQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("RefreshCarrierStatus")]
  public async Task<ActionResult> RefreshCarrierStatus([FromBody] RefreshCarrierStatusCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetValidatedOrderAddressByActiveCarrier")]
  public async Task<ActionResult> GetValidatedOrderAddressByActiveCarrier([FromBody] GetValidatedOrderAddressByActiveCarrierQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("ValidateOrderAddressForCarrier")]
  public async Task<ActionResult> ValidateOrderAddressForCarrier([FromBody] ValidateOrderAddressForCarrierQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetAllOrderStatusReport")]
  public async Task<ActionResult> GetAllOrderStatusReport([FromBody] GetAllOrderStatusReportQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("UploadPaperlessDocByCarrier")]
  public async Task<ActionResult> UploadPaperlessDocByCarrier([FromForm] UploadPaperlessDocByCarrierCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("CreateThirdPartyPickupLocation")]
  public async Task<ActionResult> CreateThirdPartyPickupLocation([FromBody] CreateThirdPartyLocationCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetAwbForCarrierByOrderNos")]
  public async Task<ActionResult> GetAwbForCarrierByOrderNos([FromQuery] GetAwbForCarrierByOrderNosQuery request, CancellationToken cancellationToken = default)
  {
    var result = await Mediator.Send(request, cancellationToken);
    var file = DirectoryHelper.GetRandomNameForPdf("Awb");
    try
    {
      return File(result.Result!, "application/pdf", file);
    }
    catch (Exception ex)
    {
      Console.WriteLine($"An error occurred: {ex.Message}");
    }
    return Ok(result);
  }
  #endregion
  [HttpPost("GenerateManifest")]
  public async Task<ActionResult> GenerateManifest([FromBody] GenerateManifestQuery request, CancellationToken cancellationToken = default)
  {
    var result = await Mediator.Send(request, cancellationToken);
    var file = DirectoryHelper.GetRandomNameForPdf("Manifest");
    try
    {
      return File(result.Result!, "application/pdf", file);
    }
    catch (Exception ex)
    {
      Console.WriteLine($"An error occurred: {ex.Message}");
    }
    return Ok(result);
  }
  #region salechannelorder
  [HttpPost("CreateOrderFromSaleChannelOrders")]
  public async Task<ActionResult> CreateOrderFromSaleChannelOrders([FromBody] CreateOrderFromSaleChannelOrdersCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion
  [HttpGet("GetAllOrderDrafts")]
  public async Task<ActionResult> GetAllOrderDrafts(CancellationToken cancellationToken = default)
  {
    GetAllOrderDraftsQuery request = new();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetOrderDraftByDraftId")]
  public async Task<ActionResult> GetOrderDraftByDraftId([FromQuery] GetOrderDraftByDraftIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("OrdersArchive")]
  public async Task<ActionResult> OrdersArchive([FromBody] OrdersArchiveCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("GetAllArchiveOrders")]
  public async Task<ActionResult> GetAllArchiveOrders([FromBody] GetAllArchiveOrdersQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("ExcelOrdersArchive")]
  public async Task<ActionResult> ExcelOrdersArchive([FromBody] ExcelExportOrdersArchiveQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return File(response!.Result?.Bytes!, ExcelExportHelper.ExcelContentType, ExcelExportHelper.GetExcelFileName("OrdersArchive"));
  }

  [HttpPost("RestoreOrderArchive")]
  public async Task<ActionResult> RestoreOrderArchive([FromBody] RestoreOrderArchiveCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetAllOrderDraftsCount")]
  public async Task<ActionResult> GetAllOrderDraftsCount(CancellationToken cancellationToken = default)
  {
    GetAllOrderDraftCountQuery request = new();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetProductNameByOrderDraftId")]
  public async Task<ActionResult> GetProductNameByOrderDraftId([FromBody] GetProductNameByOrderDraftIdCommand request, CancellationToken cancellationToken = default)
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
  [HttpGet("GetAllReturnReasonForSelection")]
  public async Task<ActionResult> GetAllClientReturnReasonForSelection([FromQuery] GetAllClientReturnReasonForSelectionQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetMapApiKey")]
  public async Task<ActionResult> GetMapApiKey()
  {
    var request = new GetMapApiKeyQuery();
    var response = await Mediator.Send(request, default);
    return Ok(response);
  }

  [HttpPost("CreatUnMappedeOrder")]
  public async Task<ActionResult> CreatUnMappedeOrder([FromBody] CreateOrderUnMappedCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetMapKey")]
  public async Task<ActionResult> GetMapKey(CancellationToken cancellationToken = default)
  {
    var request = new GetMapKeyQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("AdvanceSearchOrders")]
  public async Task<ActionResult> AdvanceSearchOrders([FromBody] Shipra.Backend.API.Application.Features.OrderFeatures.Query.AdvanceSearchOrders.AdvanceSearchOrdersQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
}
