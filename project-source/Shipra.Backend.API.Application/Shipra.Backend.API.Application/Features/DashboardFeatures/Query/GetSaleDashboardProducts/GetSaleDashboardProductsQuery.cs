using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DashboardFeatures.Query.GetSaleDashboardProducts;

public class GetSaleDashboardProductsQuery : IRequest<ServiceResultDTO>
{
  public int StoreId { get; set; }
  public int? SaleChannelConfigId { get; set; }
  public FilterModelDTO? FilterModel { get; set; }
}

public class GetSaleDashboardProductsQueryHandler : RequestHandlerBase<GetSaleDashboardProductsQuery, ServiceResultDTO>
{
  private readonly IDashboardRepository _dashboardRepository;

  public GetSaleDashboardProductsQueryHandler(IDashboardRepository dashboardRepository, IServiceProvider serviceProvider, ILogger<GetSaleDashboardProductsQueryHandler> logger) : base(serviceProvider, logger)
  {
    _dashboardRepository = dashboardRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetSaleDashboardProductsQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request.FilterModel!;
      var result = await _dashboardRepository.GetSaleDashboardProducts(
        request.StoreId,
        request.SaleChannelConfigId,
        filter.Start,
        filter.Length,
        filter.Search!,
        _currentUser.ClientIdStr!,
        filter.CreatedFrom,
        filter.CreatedTo
      );
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
