using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DocumentGenerator.ExcelReports;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Query.ExcelExportAllDeliveryTasks;

public class ExcelExportAllDeliveryTasksQueryHandler : RequestHandlerBase<ExcelExportAllDeliveryTasksQuery, ServiceResultDTOWithTypeModel<ExcelResponseModel>>
{
  private readonly IDeliveryTaskRepository _deliveryTaskRepository;

  public ExcelExportAllDeliveryTasksQueryHandler(IDeliveryTaskRepository deliveryTaskRepository, IServiceProvider serviceProvider, ILogger<ExcelExportAllDeliveryTasksQueryHandler> logger) : base(serviceProvider, logger)
  {
    _deliveryTaskRepository = deliveryTaskRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<ExcelResponseModel>> HandleRequest(ExcelExportAllDeliveryTasksQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTOWithTypeModel<ExcelResponseModel> serviceResult = new ServiceResultDTOWithTypeModel<ExcelResponseModel>();
    try
    {
      var filter = request.FilterModel!;
      var clientId = _currentUser.ClientId!.Value.ToString();
      dynamic deliveryTasks = await _deliveryTaskRepository.GetAllDeliveryTask(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search, filter.SortCol, filter.SortDir, clientId, request?.DriverAssignedStatus!, request!.CountryId, request.CarrierTrackingStatusIds, request.StoreIds, request.DeliveryTaskStatusIds, request.DriverIds, request.IncludeDriver, request.OrderAddressFilter, request.SalePersonIds, request.OrderLabels, request.DuplicateStatus, request.DeliveryNoteStatusId);
      
      var excelDeliveryTasks = new ExportToExcelDeliveryTasks();
      var data = excelDeliveryTasks.ExportToExcel(deliveryTasks.list, "Delivery Tasks Report");
      
      serviceResult = new ServiceResultDTOWithTypeModel<ExcelResponseModel>(new ExcelResponseModel { Bytes = data });
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
