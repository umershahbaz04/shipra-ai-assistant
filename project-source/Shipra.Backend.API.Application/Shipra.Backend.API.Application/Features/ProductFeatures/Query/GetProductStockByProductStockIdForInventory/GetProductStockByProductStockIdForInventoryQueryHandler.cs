using System.Net;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Query.GetProductStockByProductStockId;
public class GetProductStockByProductStockIdForInventoryQueryHandler : RequestHandlerBase<GetProductStockByProductStockIdForInventoryQuery, ServiceResultDTO>
{
  private readonly IProductRepository _productRepository;
  public GetProductStockByProductStockIdForInventoryQueryHandler(IProductRepository productRepository, IServiceProvider serviceProvider, ILogger<GetProductStockByProductStockIdForInventoryQueryHandler> logger) : base(serviceProvider, logger)
  {
    _productRepository = productRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetProductStockByProductStockIdForInventoryQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var productStock = await _productRepository.GetProductStockByIdAsync(request.ProductStockId);
      if (productStock == null)
      {
        throw new EntityNotFoundException("ProductStock ", request!.ProductStockId!);
      }
      serviceResult = new ServiceResultDTO(new
      {
        productStock.Price,
        productStock.QuantityInComing,
        productStock.QuantityAvailable,
        productStock.QuantityDamage,
        productStock.QuantityOnOrder
      });
      serviceResult.CreateSuccessResponse(HttpStatusCode.OK);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
