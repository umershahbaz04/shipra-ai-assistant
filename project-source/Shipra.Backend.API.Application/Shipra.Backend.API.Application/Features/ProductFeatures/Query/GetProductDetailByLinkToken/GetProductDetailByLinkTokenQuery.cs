using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Query.GetProductDetailByLinkToken;
public class GetProductDetailByLinkTokenQuery : IRequest<ServiceResultDTO>
{
  public string? Token { get; set; }
}
public class GetProductDetailByLinkTokenQueryHandler : RequestHandlerBase<GetProductDetailByLinkTokenQuery, ServiceResultDTO>
{
  private readonly IClientRepositoryInitializer _clientRepositoryInitializer;

  public GetProductDetailByLinkTokenQueryHandler(IClientRepositoryInitializer clientRepositoryInitializer, IServiceProvider serviceProvider, ILogger<GetProductDetailByLinkTokenQueryHandler> logger) : base(serviceProvider, logger)
  {
    _clientRepositoryInitializer = clientRepositoryInitializer;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetProductDetailByLinkTokenQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      ProductLinkToken oProductLinkToken = await _clientRepositoryInitializer.GetProductLinkTokenByToken(request.Token);

      if (oProductLinkToken == null)
      {
        serviceResult.CreateError("ProductLinkTokenNotFound", new string[] { "Sorry, the page you're looking for doesn't exist." });
        return serviceResult;
      }

      _currentUser.ClientId = oProductLinkToken.ClientId;
      _currentUser.ClientIdStr = oProductLinkToken.ClientId?.Value!.ToString();
      string? productIdStr = oProductLinkToken.ProductId?.Value.ToString();
      var product = await _clientRepositoryInitializer.GetProductByIdForTakeOrder(productIdStr!,oProductLinkToken.StoreId.GetValueOrDefault(), _currentUser.ClientIdStr!);
      if (product is not null)
      {
        serviceResult = new ServiceResultDTO(product); 
      }
      else
      {
        serviceResult.CreateError("ProductNotFound", new string[] { "Sorry, the page you're looking for doesn't exist." });
      }
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
