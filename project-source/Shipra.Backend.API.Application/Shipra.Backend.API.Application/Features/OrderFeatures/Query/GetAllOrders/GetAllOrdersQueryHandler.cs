using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetAllOrders;

public class GetAllOrdersQueryHandler : RequestHandlerBase<GetAllOrdersQuery, ServiceResultDTO>
{
  private readonly IOrderRepository _orderRepository;

  public GetAllOrdersQueryHandler(IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<GetAllOrdersQueryHandler> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;  

  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllOrdersQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var filter = request.FilterModel!;
      var clientId = _currentUser.ClientId!.Value.ToString();
      dynamic data = await _orderRepository.GetAllOrders(filter.CreatedFrom, filter.CreatedTo,request.OrderFromDate,request.OrderToDate, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!, clientId, request.StoreId, request.OrderTypeId, request.CarrierId, request.FullFillmentStatusId, request.PaymentStatusId, request.PaymentMethodId, request.StationId, request.ReadyForAssignment, request.CarrierAssign,request.SaleChannelConfigIds,request.SalePersonIds,request.CountryId,request.CarrierTrackingStatusIds,request.OrderAddressFilter,request.OrderLabels, request.IsWithoutStation);

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
}
