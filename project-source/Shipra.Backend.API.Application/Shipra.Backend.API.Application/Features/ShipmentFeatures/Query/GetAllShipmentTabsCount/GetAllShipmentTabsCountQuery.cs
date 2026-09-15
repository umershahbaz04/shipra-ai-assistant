using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Application.DTOs.ShipmentUseCase;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ShipmentFeatures.Query.GetAllShipmentTabsCount;
public class GetAllShipmentTabsCountQuery : ShipmentFilters,IRequest<ServiceResultDTO>
{
   
}
public class GetAllShipmentTabsCountQueryHandler : RequestHandlerBase<GetAllShipmentTabsCountQuery, ServiceResultDTO>
{
  private readonly IShipmentRepository _shipmentRepository;

  public GetAllShipmentTabsCountQueryHandler(IShipmentRepository shipmentRepository, IServiceProvider serviceProvider, ILogger<GetAllShipmentTabsCountQueryHandler> logger) : base(serviceProvider, logger)
  {
    _shipmentRepository = shipmentRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllShipmentTabsCountQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request.FilterModel!;

      dynamic result = await _shipmentRepository.GetAllShipmentTabsCount(filter.CreatedFrom, filter.CreatedTo,request.OrderFromDate,request.OrderToDate, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!, _currentUser!.ClientIdStr!, request.StoreId, request.OrderTypeId, request.CarrierId, request.FullFillmentStatusId, request.PaymentStatusId, request.PaymentMethodId, request.StationId, request.CarrierTrackingStatusIds,request.SaleChannelConfigIds,request.SalePersonIds,request.CountryId,request.OrderAddressFilter);

      serviceResult = new ServiceResultDTO(result);
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
