using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.StoreFeatures.Query.GetAllStoresQuery;
public class GetAllStoresQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
}
public class GetAllStoresQueryHandler : RequestHandlerBase<GetAllStoresQuery, ServiceResultDTO>
{
  private readonly IStoreRepository _storeRepository;
  public GetAllStoresQueryHandler(IStoreRepository storeRepository, IServiceProvider serviceProvider, ILogger<GetAllStoresQueryHandler> logger) : base(serviceProvider, logger)
  {
    _storeRepository = storeRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllStoresQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request.FilterModel!;
      var clientId = _currentUser.ClientId!.Value.ToString();
      var data = await _storeRepository.GetAllStore(clientId, filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!);
       
      serviceResult = new ServiceResultDTO(data!);
      serviceResult.CreateSuccessResponse();
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
