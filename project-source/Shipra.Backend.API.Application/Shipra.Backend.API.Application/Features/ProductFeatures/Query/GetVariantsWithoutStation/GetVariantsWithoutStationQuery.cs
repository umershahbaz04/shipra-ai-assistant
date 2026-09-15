using MediatR;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Query.GetVariantsWithoutStation;

public class GetVariantsWithoutStationQuery : IRequest<ServiceResultDTO>
{
}

public class GetVariantsWithoutStationQueryHandler : RequestHandlerBase<GetVariantsWithoutStationQuery, ServiceResultDTO>
{
  private readonly IProductRepository _productRepository;

  public GetVariantsWithoutStationQueryHandler(
      IProductRepository productRepository,
      IServiceProvider serviceProvider,
      ILogger<GetVariantsWithoutStationQueryHandler> logger) : base(serviceProvider, logger)
  {
    _productRepository = productRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetVariantsWithoutStationQuery request, CancellationToken cancellationToken)
  {
    var response = new ServiceResultDTO();
    try
    {
      var variantsDynamic = await _productRepository.GetVariantsWithoutStationAsync(_currentUser.ClientId!.Value.ToString());
      response = new ServiceResultDTO(variantsDynamic);
      return response;
    }
    catch (Exception ex)
    {
      response.CreateErrorResponse(ex);
      throw;
    }
  }
}
