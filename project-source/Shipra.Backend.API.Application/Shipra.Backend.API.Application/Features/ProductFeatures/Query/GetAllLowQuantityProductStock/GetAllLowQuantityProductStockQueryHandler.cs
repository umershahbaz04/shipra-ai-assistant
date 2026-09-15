using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DocumentGenerator.ExcelReports;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Query.GetAllLowQuantityProductStock;
public class GetAllLowQuantityProductStockQueryHandler : RequestHandlerBase<GetAllLowQuantityProductStockQuery, ServiceResultDTO>
{
  private readonly IProductRepository _productRepository;

  public GetAllLowQuantityProductStockQueryHandler(IProductRepository productRepository, IServiceProvider serviceProvider, ILogger<GetAllLowQuantityProductStockQueryHandler> logger) : base(serviceProvider, logger)
  {
    _productRepository = productRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllLowQuantityProductStockQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request.FilterModel!;

      dynamic data = await _productRepository.GetAllLowQuantityProductStock(request.StoreId, request.ProductStationId, request.IsActive,request.AvailableQty, _currentUser.ClientIdStr!, filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!);
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
