using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DocumentGenerator.ExcelReports;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.ExcelExportAllOrders;

public class ExcelExportAllOrdersQueryHandler : RequestHandlerBase<ExcelExportAllOrdersQuery, ServiceResultDTOWithTypeModel<ExcelResponseModel>>
{
  private readonly IOrderRepository _orderRepository;

  public ExcelExportAllOrdersQueryHandler(IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<ExcelExportAllOrdersQueryHandler> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<ExcelResponseModel>> HandleRequest(ExcelExportAllOrdersQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTOWithTypeModel<ExcelResponseModel> serviceResult = new ServiceResultDTOWithTypeModel<ExcelResponseModel>();
    try
    {
      var filter = request.FilterModel!;
      var clientId = _currentUser.ClientId!.Value.ToString();
      dynamic orders = await _orderRepository.GetAllOrders(filter.CreatedFrom, filter.CreatedTo,request.OrderFromDate,request.OrderToDate, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!, clientId, request.StoreId, request.OrderTypeId, request.CarrierId, request.FullFillmentStatusId, request.PaymentStatusId, request.PaymentMethodId, request.StationId, request.ReadyForAssignment, request.CarrierAssign,request.SaleChannelConfigIds,request.SalePersonIds,request.CountryId,request.CarrierTrackingStatusIds,request.OrderAddressFilter,request.OrderLabels, request.IsWithoutStation);
      var excelOrders = new ExportToExcelOrders();


      var data = excelOrders.ExportToExcel(orders.list, "Orders Report");
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
