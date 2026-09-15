using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Query.GetProductStockPriceByProductStockID;
public class GetProductStockPriceByProductStockIdQuery : IRequest<ServiceResultDTO>
{
  public long? ProductStockId { get; set; }
}
public class GetProductStockPriceByProductStockIdQueryHandler : RequestHandlerBase<GetProductStockPriceByProductStockIdQuery, ServiceResultDTO>
{
  private readonly IProductRepository _productRepository;
  public GetProductStockPriceByProductStockIdQueryHandler(IProductRepository productRepository, IServiceProvider serviceProvider, ILogger<GetProductStockPriceByProductStockIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _productRepository = productRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(GetProductStockPriceByProductStockIdQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var target = await _productRepository.GetProductStockPriceByProductStockId(request?.ProductStockId);
      if (target == null)
      {
        throw new EntityNotFoundException("ProductStock Price ", request!.ProductStockId!);
      }

      //var model = _mapper.Map<ProductStockReuqestModel>(target);
      var stock = target as ProductStock;
      serviceResult = new ServiceResultDTO(new { price = stock!.Price });
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
