using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.StationLookupUserCase;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Application.Features.StationLookupFeatures.Query.GetAllStationLookupQuery;
public class GetAllStationLookupQuery : IRequest<ServiceResultDTOWithTypeModel<List<StationLookupResponseModel>>>
{
}

public class GetAllStationLookupQueryHandler : RequestHandlerBase<GetAllStationLookupQuery, ServiceResultDTOWithTypeModel<List<StationLookupResponseModel>>>
{
  private readonly IStationLookupRepository _stationLookupRepository;

  public GetAllStationLookupQueryHandler(IStationLookupRepository stationLookupRepository, IServiceProvider serviceProvider, ILogger<GetAllStationLookupQueryHandler> logger) : base(serviceProvider, logger)
  {
    _stationLookupRepository = stationLookupRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<List<StationLookupResponseModel>>> HandleRequest(GetAllStationLookupQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTOWithTypeModel<List<StationLookupResponseModel>> serviceResult = new ServiceResultDTOWithTypeModel<List<StationLookupResponseModel>>();

    try
    {
      var oStationList = await _stationLookupRepository.GetAllStationLookup(_currentUser.ClientId!);
      oStationList!.Add(ProductStation.AddDefault());
      if (oStationList is not null)
      {
        var responseDto = oStationList!.Select(x => 
        new StationLookupResponseModel() {
          ProductStationId = x.ProductStationId,
          Sname = x.IsDefault == true ? $"{x.Name} (Default)" : x.Name
        })
          .OrderBy(x => x.ProductStationId).ToList();
        serviceResult = new ServiceResultDTOWithTypeModel<List<StationLookupResponseModel>>(responseDto);
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

