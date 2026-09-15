using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ShipmentFeatures.Query.GetSettingOperationDashboard;
public class GetShipmentTabsCountConfigQueryHandler : RequestHandlerBase<GetShipmentTabsCountConfigQuery, ServiceResultDTO>
{
  private readonly IShipmentRepository _shipmentRepository;
  public GetShipmentTabsCountConfigQueryHandler(IShipmentRepository shipmentRepository, IServiceProvider serviceProvider, ILogger<GetShipmentTabsCountConfigQueryHandler> logger) : base(serviceProvider, logger)
  {
    _shipmentRepository = shipmentRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(GetShipmentTabsCountConfigQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var settingOpDashboard = await _shipmentRepository.GetAllShipmentGridClientSettingForDashboard(_currentUser.ClientIdStr!);
      serviceResult = new ServiceResultDTO(settingOpDashboard);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
