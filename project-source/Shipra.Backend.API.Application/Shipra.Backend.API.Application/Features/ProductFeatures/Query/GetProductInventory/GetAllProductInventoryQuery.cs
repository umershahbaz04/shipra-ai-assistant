using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Application.DTOs.ProductUseCase.Request;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Query.GetProductInventory;
public class GetAllProductInventoryQuery : ProductInventoryFilterModel,IRequest<ServiceResultDTO>
{
 
}

public class GetAllProductInventoryQueryHandler : RequestHandlerBase<GetAllProductInventoryQuery, ServiceResultDTO>
{
  private readonly IProductRepository _productRepository;

  public GetAllProductInventoryQueryHandler(IProductRepository productRepository,IServiceProvider serviceProvider, ILogger<GetAllProductInventoryQueryHandler> logger) : base(serviceProvider, logger)
  {
    _productRepository = productRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllProductInventoryQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request.FilterModel!;

      dynamic data = await _productRepository.GetAllProductInventoryAsync(request.StoreId, request.ProductStationId, request.IsActive,request.IsAvailable,request.AvailableQty, _currentUser.ClientIdStr!, filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!);
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
