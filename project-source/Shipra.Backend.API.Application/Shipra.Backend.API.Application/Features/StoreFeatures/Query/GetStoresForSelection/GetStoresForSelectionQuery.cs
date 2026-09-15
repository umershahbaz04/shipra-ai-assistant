using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.StoresAggregate;

namespace Shipra.Backend.API.Application.Features.StoreFeatures.Query.GetStoresFroSelection;
public class GetStoresForSelectionQuery : IRequest<ServiceResultDTO>
{
}
public class GetStoresForSelectionQueryHandler : RequestHandlerBase<GetStoresForSelectionQuery, ServiceResultDTO>
{
  private readonly IStoreRepository _storeRepository;
  public GetStoresForSelectionQueryHandler(IStoreRepository storeRepository, IServiceProvider serviceProvider, ILogger<GetStoresForSelectionQueryHandler> logger) : base(serviceProvider, logger)
  {
    _storeRepository = storeRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(GetStoresForSelectionQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var storeList = await _storeRepository.GetStoresForSelection(_currentUser.ClientId!);
      if (storeList is not null)
      {
        storeList?.Add(Store.AddDefault());
        var seperatedList = storeList!.Select(x => new { StoreId = x.StoreId, StoreName = (x.StoreName + (x.IsDefault.GetValueOrDefault(false) ? " (Default)" : "")) }).OrderBy(x => x.StoreId).ToList();

        serviceResult = new ServiceResultDTO(seperatedList);
        serviceResult.CreateSuccessResponse();
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
