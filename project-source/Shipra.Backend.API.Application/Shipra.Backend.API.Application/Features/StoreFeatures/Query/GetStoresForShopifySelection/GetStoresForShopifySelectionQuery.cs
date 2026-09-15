using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.StoresAggregate;

namespace Shipra.Backend.API.Application.Features.StoreFeatures.Query.GetStoresForShopifySelection;

public class GetStoresForShopifySelectionQuery : IRequest<ServiceResultDTO>
{
  public string? ClientId { get; set; }
  public string? SecretKey { get; set; }
}
public class GetStoresForShopifySelectionQueryHandler : RequestHandlerBase<GetStoresForShopifySelectionQuery, ServiceResultDTO>
{
  private readonly IShopifyPluginRepository _shopifyPluginRepository;

  public GetStoresForShopifySelectionQueryHandler(IShopifyPluginRepository shopifyPluginRepository, IServiceProvider serviceProvider, ILogger<GetStoresForShopifySelectionQueryHandler> logger) : base(serviceProvider, logger)
  {
    _shopifyPluginRepository = shopifyPluginRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(GetStoresForShopifySelectionQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var clientId = new ClientId(new Guid(request.ClientId!));
      var oClient = await _shopifyPluginRepository.GetClientById(clientId);
      if (oClient is not null)
      {
        if (oClient.SecretKey == request.SecretKey)
        {
          var storeList = await _shopifyPluginRepository.GetStoresForSelection(clientId);
          if (storeList is not null)
          {
            storeList?.Add(Store.AddDefault());
            var seperatedList = storeList!.Select(x => new { StoreId = x.StoreId, StoreName = (x.StoreName + (x.IsDefault.GetValueOrDefault(false) ? " (Default)" : "")) }).OrderBy(x => x.StoreId).ToList();

            serviceResult = new ServiceResultDTO(seperatedList);
            serviceResult.CreateSuccessResponse();
          }
          return serviceResult;
        }
        else
        {
          throw new ShipraApplicationException(System.Net.HttpStatusCode.ExpectationFailed, "Invalid User secret key.");
        }
      }
      else
      {
        throw new ShipraApplicationException(System.Net.HttpStatusCode.ExpectationFailed, "Invalid User Entity or Not found.");
      }
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
