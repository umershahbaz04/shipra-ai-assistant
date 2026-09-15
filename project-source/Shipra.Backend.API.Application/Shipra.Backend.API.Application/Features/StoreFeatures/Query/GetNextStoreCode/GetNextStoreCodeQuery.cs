using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.StoreFeatures.Query.GetNextStoreCode;
public class GetNextStoreCodeQuery : IRequest<ServiceResultDTO>
{
}
public class GetNextStoreCodeQueryHandler : RequestHandlerBase<GetNextStoreCodeQuery, ServiceResultDTO>
{
  private readonly IStoreRepository _storeRepository;

  public GetNextStoreCodeQueryHandler(IStoreRepository storeRepository, IServiceProvider serviceProvider, ILogger<GetNextStoreCodeQueryHandler> logger) : base(serviceProvider, logger)
  {
    _storeRepository = storeRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetNextStoreCodeQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var storeCode = await _storeRepository.GetClientNextStoreCode(_currentUser.ClientId!);
      serviceResult = new ServiceResultDTO(new { storeCode });
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
