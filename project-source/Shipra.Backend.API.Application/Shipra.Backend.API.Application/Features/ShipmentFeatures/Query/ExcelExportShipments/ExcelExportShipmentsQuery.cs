using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DocumentGenerator.ExcelReports;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Application.DTOs.ShipmentUseCase;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ShipmentFeatures.Query.ExcelExportsShipments;
public class ExcelExportShipmentsQuery : ShipmentFilters,IRequest<ServiceResultDTOWithTypeModel<ExcelResponseModel>>
{ 
}
public class ExcelExportShipmentsQueryHandler : RequestHandlerBase<ExcelExportShipmentsQuery, ServiceResultDTOWithTypeModel<ExcelResponseModel>>
{
  private readonly IShipmentRepository _shipmentRepository;

  public ExcelExportShipmentsQueryHandler(IShipmentRepository shipmentRepository, IServiceProvider serviceProvider, ILogger<ExcelExportShipmentsQueryHandler> logger) : base(serviceProvider, logger)
  {
    _shipmentRepository = shipmentRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<ExcelResponseModel>> HandleRequest(ExcelExportShipmentsQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTOWithTypeModel<ExcelResponseModel> serviceResult = new ServiceResultDTOWithTypeModel<ExcelResponseModel>();
    try
    {
      var filter = request.FilterModel!;
      var clientId = _currentUser.ClientId!.Value.ToString();
      dynamic shipments = await _shipmentRepository.GetAllShipments(filter.CreatedFrom, filter.CreatedTo,request.OrderFromDate,request.OrderToDate, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!, clientId, request.StoreId, request.OrderTypeId, request.CarrierId, request.FullFillmentStatusId, request.PaymentStatusId, request.PaymentMethodId, request.StationId, request.CarrierTrackingStatusIds,request.SaleChannelConfigIds,request.SalePersonIds,request.CountryId,request.OrderAddressFilter);
      var excelShipments = new ExportToExcelShipments();


      var data = excelShipments.ExportToExcel(shipments.list, "Shipments Report");
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
