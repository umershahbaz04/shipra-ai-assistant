using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DocumentGenerator.ExcelReports;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Query.ExcelExportCODCollectionsMyCarrier;
public class ExcelExportCODCollectionsMyCarrierQueryHandler : RequestHandlerBase<ExcelExportCODCollectionsMyCarrierQuery, ServiceResultDTO>
{
  private readonly IDeliveryTaskRepository _deliveryTaskRepository;

  public ExcelExportCODCollectionsMyCarrierQueryHandler(IDeliveryTaskRepository deliveryTaskRepository, IServiceProvider serviceProvider, ILogger<ExcelExportCODCollectionsMyCarrierQueryHandler> logger) : base(serviceProvider, logger)
  {
    _deliveryTaskRepository = deliveryTaskRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(ExcelExportCODCollectionsMyCarrierQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request.FilterModel!;
      var oCODCollectionList = await _deliveryTaskRepository.GetAllCODCollectionsMyCarrier(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!, _currentUser.ClientId!.Value.ToString(), request.StoreId, request.CarrierIds, request.OrderTypeId);
      var excelShipments = new ExportToExcelCodPending();

      var data = excelShipments.ExportToExcel(oCODCollectionList.list, "COD Collection");
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
