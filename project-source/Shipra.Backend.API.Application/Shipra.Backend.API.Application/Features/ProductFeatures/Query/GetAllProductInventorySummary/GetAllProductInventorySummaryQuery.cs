using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Application.DTOs.ProductUseCase.Request;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Query.GetAllProductInventorySummary;
public class GetAllProductInventorySummaryQuery : ProductInventoryFilterModel,IRequest<ServiceResultDTO>
{ 
}
public class GetAllProductInventorySummaryQueryHandler : RequestHandlerBase<GetAllProductInventorySummaryQuery, ServiceResultDTO>
{
  private readonly IProductRepository _productRepository;

  public GetAllProductInventorySummaryQueryHandler(IProductRepository productRepository, IServiceProvider serviceProvider, ILogger<GetAllProductInventorySummaryQueryHandler> logger) : base(serviceProvider, logger)
  {
    _productRepository = productRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllProductInventorySummaryQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request.FilterModel!;
      var data = await _productRepository.GetAllProductInventorySummaryAsync(request.StoreId, request.ProductStationId, request.IsActive, request.IsAvailable, request.AvailableQty, _currentUser.ClientIdStr!, filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!);

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
