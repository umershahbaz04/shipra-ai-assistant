using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CommonFeatures.Query.GetAllCarrierTrackingStatusLookup;

public class GetAllClientCarrierTrackingStatusLookupQueryHandler : RequestHandlerBase<GetAllClientCarrierTrackingStatusLookupQuery, ServiceResultDTO>
{
  private readonly ICommonLookupRepository _commonLookupRepository;

  public GetAllClientCarrierTrackingStatusLookupQueryHandler(ICommonLookupRepository commonLookupRepository, IServiceProvider serviceProvider, ILogger<GetAllClientCarrierTrackingStatusLookupQueryHandler> logger) : base(serviceProvider, logger)
  {
    _commonLookupRepository = commonLookupRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(GetAllClientCarrierTrackingStatusLookupQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var oCarrierTrackingData = await _commonLookupRepository.GetAllClientCarrierTrackingStatus(_currentUser.ClientId!);
      oCarrierTrackingData!.Add(ClientCarrierTrackingStatus.AddDefault());

      var selectedAttributes = oCarrierTrackingData!.Select(x => new { x.CarrierTrackingStatusId, x.TrackingStatus }).OrderBy(x => x.CarrierTrackingStatusId).ToList();
      serviceResult = new ServiceResultDTO(selectedAttributes!); 
      return serviceResult!;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
