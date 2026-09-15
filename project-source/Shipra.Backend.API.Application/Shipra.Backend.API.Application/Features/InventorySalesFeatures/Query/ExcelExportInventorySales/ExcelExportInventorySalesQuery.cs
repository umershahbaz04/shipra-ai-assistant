using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DocumentGenerator.ExcelReports;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.InventorySalesFeatures.Query.ExcelExportSalesInventory;
public class ExcelExportInventorySalesQuery : IRequest<ServiceResultDTOWithTypeModel<ExcelResponseModel>>
{
  public FilterModelDTO? FilterModel { get; set; }
  public string? ProductStationIds { get; set; }
  public string? ProductSKUs { get; set; }
  public string? TrackingStatusID { get; set; }
  public string? saleChannelConfigIds { get; set; }
  public string? StoreIds { get; set; }
  public bool? IsFulfilled { get; set; }
  public bool? IsInTransit { get; set; }
  public string? RegionIds { get; set; }
}
public class ExcelExportInventorySalesQueryHandler : RequestHandlerBase<ExcelExportInventorySalesQuery, ServiceResultDTOWithTypeModel<ExcelResponseModel>>
{
  private readonly IInventoryRepository _inventoryRepository;

  public ExcelExportInventorySalesQueryHandler(IInventoryRepository inventoryRepository, IServiceProvider serviceProvider, ILogger<ExcelExportInventorySalesQueryHandler> logger) : base(serviceProvider, logger)
  {
    _inventoryRepository = inventoryRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<ExcelResponseModel>> HandleRequest(ExcelExportInventorySalesQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTOWithTypeModel<ExcelResponseModel> serviceResult = new ServiceResultDTOWithTypeModel<ExcelResponseModel>();
    try
    {
      var filter = request.FilterModel!;
      var inventorySales = await _inventoryRepository.GetAllInventorySales(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!, request?.ProductStationIds, request?.ProductSKUs, request?.TrackingStatusID!, request?.IsFulfilled, request?.IsInTransit, request?.RegionIds, request!.saleChannelConfigIds, request.StoreIds!, _currentUser.ClientIdStr!);
      var inventorySalesExcel = new ExportToExcelInventorySales();
      var data = inventorySalesExcel.ExportToExcel(inventorySales?.list, "Inventory Sales Summary Report");
      serviceResult = new ServiceResultDTOWithTypeModel<ExcelResponseModel>(new ExcelResponseModel() { Bytes = data });

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
