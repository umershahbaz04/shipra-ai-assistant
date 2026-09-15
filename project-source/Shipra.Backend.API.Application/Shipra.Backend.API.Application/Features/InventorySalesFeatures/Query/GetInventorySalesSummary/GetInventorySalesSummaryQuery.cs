using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.InventorySalesFeatures.Query.GetInventorySalesSummary;
public class GetInventorySalesSummaryQuery : IRequest<ServiceResultDTO>
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
public class GetInventorySalesSummaryQueryHandler : RequestHandlerBase<GetInventorySalesSummaryQuery, ServiceResultDTO>
{
  private readonly IInventoryRepository _inventoryRepository;
  public GetInventorySalesSummaryQueryHandler(IInventoryRepository inventoryRepository, IServiceProvider serviceProvider, ILogger<GetInventorySalesSummaryQueryHandler> logger) : base(serviceProvider, logger)
  {
    _inventoryRepository = inventoryRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetInventorySalesSummaryQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request.FilterModel!;
      dynamic data = await _inventoryRepository.GetInventorySalesSummary(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!, request.ProductStationIds, request.ProductSKUs, request.TrackingStatusID, request.IsFulfilled, request.IsInTransit, request.RegionIds, request.saleChannelConfigIds, request.StoreIds!, _currentUser.ClientIdStr!);
      serviceResult = new ServiceResultDTO(data);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
