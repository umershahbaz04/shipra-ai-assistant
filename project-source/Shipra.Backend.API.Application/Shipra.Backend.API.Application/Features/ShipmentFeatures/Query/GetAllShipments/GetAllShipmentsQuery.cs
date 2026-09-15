using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ShipmentUseCase;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.SettingOperationDashboardAggregate;

namespace Shipra.Backend.API.Application.Features.ShipmentFeatures.Query.GetAllShipmentsQuery;
public class GetAllShipmentsQuery : ShipmentFilters, IRequest<ServiceResultDTO>
{
}
public class GetAllShipmentsQueryHandler : RequestHandlerBase<GetAllShipmentsQuery, ServiceResultDTO>
{
  private readonly IShipmentRepository _shipmentRepository;

  public GetAllShipmentsQueryHandler(IShipmentRepository shipmentRepository, IServiceProvider serviceProvider, ILogger<GetAllShipmentsQueryHandler> logger) : base(serviceProvider, logger)
  {
    _shipmentRepository = shipmentRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllShipmentsQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();

    try
    {
      var filter = request.FilterModel!;
      var clientId = _currentUser.ClientId!.Value.ToString();
      if (true)
      {
        int shipmentGridColumnId = 1;
        ShipmentGridColumn oShipmentGridColumn = await _shipmentRepository.GetShipmentGridColumnById(shipmentGridColumnId, _currentUser.ClientId);
        if (oShipmentGridColumn is not null)
        {
          //var dt = GetTrackingStatusId(oShipmentGridColumn,_currentUser.ClientId!);
          if (oShipmentGridColumn.ColumnName == "TO BE DELIVERED")
          {
            var dta = await _shipmentRepository.GetAllShipmentGridClientSetting(_currentUser.ClientId);
            //var notCompleted = dta.Select(x => x.)
          }
        }
      }
      dynamic data = await _shipmentRepository.GetAllShipments(filter.CreatedFrom, filter.CreatedTo,request.OrderFromDate,request.OrderToDate, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!, clientId, request.StoreId, request.OrderTypeId, request.CarrierId, request.FullFillmentStatusId, request.PaymentStatusId, request.PaymentMethodId, request.StationId, request.CarrierTrackingStatusIds, request.SaleChannelConfigIds, request.SalePersonIds, request.CountryId,request.OrderAddressFilter);

      serviceResult = new ServiceResultDTO(data);
      serviceResult.CreateSuccessResponse();
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }

  private object GetTrackingStatusId(ShipmentGridColumn oShipmentGridColumn, Core.ClientAggregate.ClientId clientId)
  {
    throw new NotImplementedException();
  }
}
