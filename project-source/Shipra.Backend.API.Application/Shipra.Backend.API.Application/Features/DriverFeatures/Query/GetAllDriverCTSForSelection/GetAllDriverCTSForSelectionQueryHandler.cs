using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DriverFeatures.Query.GetAllDriverCTSForSelection;
public class GetAllDriverCTSForSelectionQueryHandler : RequestHandlerBase<GetAllDriverCTSForSelectionQuery, ServiceResultDTO>
{
  private readonly IDriverRepository _driverRepository;
  public GetAllDriverCTSForSelectionQueryHandler(IDriverRepository driverRepository, IServiceProvider serviceProvider, ILogger<GetAllDriverCTSForSelectionQueryHandler> logger) : base(serviceProvider, logger)
  {
    _driverRepository = driverRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllDriverCTSForSelectionQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var oCarrierTrackingData = await _driverRepository.GetAllDriverCTSSetting(_currentUser!.ClientIdStr!);
      var castedList = (IEnumerable<dynamic>)oCarrierTrackingData!.list;
      var selectedAttributes = castedList!.Select(x => new { x.CarrierTrackingStatusId, x.TrackingStatus });
      serviceResult = new ServiceResultDTO(selectedAttributes!);
      serviceResult.CreateSuccessResponse(System.Net.HttpStatusCode.OK);
      return serviceResult!;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
