using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DocumentGenerator.ExcelReports;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.AccountFeature.Query.ExcelExportCodPending;
public class ExcelExportCodPendingQueryHandler : RequestHandlerBase<ExcelExportCodPendingQuery, ServiceResultDTO>
{
  private readonly IOrderRepository _orderRepository;

  public ExcelExportCodPendingQueryHandler(IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<ExcelExportCodPendingQueryHandler> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(ExcelExportCodPendingQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request.FilterModel!;
      var clientId = _currentUser.ClientId!.Value.ToString();
      dynamic reportData = await _orderRepository.GetAllCODPendings(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!, clientId, request.StoreId, request.CarrierIds, request.OrderTypeId);
      var excelShipments = new ExportToExcelCodPending();

      var data = excelShipments.ExportToExcel(reportData.list, "COD Pending Report");
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
