using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DocumentGenerator.ExcelReports;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ReturnReportFeature.Query.ExcelExportReturnReportById;
public class ExcelExportReturnReportByIdQuery : IRequest<ServiceResultDTO>
{
  public string? CarrierRrid { get; set; }
}
public class ExcelExportReturnReportByIdQueryHandler : RequestHandlerBase<ExcelExportReturnReportByIdQuery, ServiceResultDTO>
{
  private readonly ICarrierReturnReport _carrierReturnReport;

  public ExcelExportReturnReportByIdQueryHandler(ICarrierReturnReport carrierReturnReport, IServiceProvider serviceProvider, ILogger<ExcelExportReturnReportByIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _carrierReturnReport = carrierReturnReport;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(ExcelExportReturnReportByIdQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var clientId = _currentUser.ClientId!.Value.ToString();
      dynamic reportData = await _carrierReturnReport.GetShipmentsByReturnReportId(request.CarrierRrid!, clientId);


      var excelShipments = new ExportToExcelCommon();
      string[] columnsNotToTake = { "OrderId", "RowNum", "TotalCount", "ItemValue", "Discount", "VAT", "StoreImage", "ClientName" };
      var data = excelShipments.ExportToExcelWithDynamicList(reportData.list, "Return Report", columnsNotToTake);
      serviceResult = new ServiceResultDTO(new ExcelResponseModel { Bytes = data });
      serviceResult.CreateSuccessResponse();
      return serviceResult; 
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
