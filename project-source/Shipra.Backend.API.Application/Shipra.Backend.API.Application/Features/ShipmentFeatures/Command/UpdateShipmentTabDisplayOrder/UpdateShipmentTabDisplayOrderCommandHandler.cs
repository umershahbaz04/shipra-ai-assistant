using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.SettingOperationDashboardAggregate;

namespace Shipra.Backend.API.Application.Features.ShipmentFeatures.Command.UpdateShipmentTabDisplayOrder;

public class UpdateShipmentTabDisplayOrderCommandHandler : RequestHandlerBase<UpdateShipmentTabDisplayOrderCommand, ServiceResultDTO>
{
  private readonly IShipmentRepository _shipmentRepository;

  public UpdateShipmentTabDisplayOrderCommandHandler(IShipmentRepository shipmentRepository,IServiceProvider serviceProvider, ILogger<UpdateShipmentTabDisplayOrderCommandHandler> logger) : base(serviceProvider, logger)
  {
    _shipmentRepository = shipmentRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdateShipmentTabDisplayOrderCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      foreach (var item in request.list!)
      {
        ShipmentGridColumn oShipmentGridColumn = await _shipmentRepository.GetShipmentGridColumnById(item.ShipmentGridColumnId,_currentUser.ClientId!);
        oShipmentGridColumn.UpdateDisplayOrder(item.DisplayOrder, _currentUser.EmployeeId!);

        bool isUpdated = await _shipmentRepository.UpdateShipmentGridColumn(oShipmentGridColumn);
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
