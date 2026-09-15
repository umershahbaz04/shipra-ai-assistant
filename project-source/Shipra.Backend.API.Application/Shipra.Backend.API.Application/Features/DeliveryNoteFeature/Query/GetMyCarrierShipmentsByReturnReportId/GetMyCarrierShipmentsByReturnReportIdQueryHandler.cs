using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetMyCarrierShipmentsByReturnReportNo;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetMyCarrierShipmentsByReturnReportId;
public class GetMyCarrierShipmentsByReturnReportIdQueryHandler : RequestHandlerBase<GetMyCarrierShipmentsByReturnReportIdQuery, ServiceResultDTO>
{
  private readonly ICarrierReturnReport _carrierReturnReport;

  public GetMyCarrierShipmentsByReturnReportIdQueryHandler(ICarrierReturnReport carrierReturnReport, IServiceProvider serviceProvider, ILogger<GetMyCarrierShipmentsByReturnReportIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _carrierReturnReport = carrierReturnReport;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetMyCarrierShipmentsByReturnReportIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      dynamic data = await _carrierReturnReport.GetShipmentsByReturnReportId(request.CarrierRRId!, _currentUser.ClientId!.Value.ToString());

      serviceResult = new ServiceResultDTO(data);
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
