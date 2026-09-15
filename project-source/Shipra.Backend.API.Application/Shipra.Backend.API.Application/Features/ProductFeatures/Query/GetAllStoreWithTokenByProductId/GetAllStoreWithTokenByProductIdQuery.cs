using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Query.GetAllStoreWithTokenByProductId;
public class GetAllStoreWithTokenByProductIdQuery : IRequest<ServiceResultDTO>
{
  public string? ProductId { get; set; }
}
public class GetAllStoreWithTokenByProductIdQueryHandler : RequestHandlerBase<GetAllStoreWithTokenByProductIdQuery, ServiceResultDTO>
{
  private readonly IProductRepository _productRepository;

  public GetAllStoreWithTokenByProductIdQueryHandler(IProductRepository productRepository, IServiceProvider serviceProvider, ILogger<GetAllStoreWithTokenByProductIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _productRepository = productRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllStoreWithTokenByProductIdQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var product = await _productRepository.GetAllStoreWithTokenByProductId(request.ProductId!, _currentUser.ClientIdStr!);
      serviceResult = new ServiceResultDTO(product);
      return serviceResult;

    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }

  }
}
