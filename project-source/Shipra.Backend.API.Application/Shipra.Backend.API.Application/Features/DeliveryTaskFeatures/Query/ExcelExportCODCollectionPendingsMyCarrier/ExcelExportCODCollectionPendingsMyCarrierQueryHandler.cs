using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DocumentGenerator.ExcelReports;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Query.ExcelExportCODCollectionPendingsMyCarrier;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Query.ExcelExportCOdCollectionPendingsMyCarrier;
public class ExcelExportCODCollectionPendingsMyCarrierQueryHandler : RequestHandlerBase<ExcelExportCODCollectionPendingsMyCarrierQuery, ServiceResultDTO>
{
  private readonly IDeliveryTaskRepository _deliveryTaskRepository;

  public ExcelExportCODCollectionPendingsMyCarrierQueryHandler(IDeliveryTaskRepository deliveryTaskRepository, IServiceProvider serviceProvider, ILogger<ExcelExportCODCollectionPendingsMyCarrierQueryHandler> logger) : base(serviceProvider, logger)
  {
    _deliveryTaskRepository = deliveryTaskRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(ExcelExportCODCollectionPendingsMyCarrierQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request.FilterModel!;
      var oCODList = await _deliveryTaskRepository.GetAllCODCollectionPendingsMyCarrier(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!, _currentUser.ClientId!.Value.ToString(), request.StoreId, request.OrderTypeId);
      var excelShipments = new ExportToExcelCodPending();

      var data = excelShipments.ExportToExcel(oCODList.list, "COD Collection Pendings");
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
