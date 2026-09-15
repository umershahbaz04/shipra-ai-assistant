using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.SettingOperationDashboardAggregate;

namespace Shipra.Backend.API.Application.Features.ShipmentFeatures.Command.CreateShipmentGridColumn;

public class CreateShipmentGridColumnCommandHandler : RequestHandlerBase<CreateShipmentGridColumnCommand, ServiceResultDTO>
{
  private readonly IShipmentRepository _shipmentRepository;

  public CreateShipmentGridColumnCommandHandler(IShipmentRepository shipmentRepository, IServiceProvider serviceProvider, ILogger<CreateShipmentGridColumnCommandHandler> logger) : base(serviceProvider, logger)
  {
    _shipmentRepository = shipmentRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateShipmentGridColumnCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      //check name already exist 
      ShipmentGridColumn oExisShipmentGridColumn = await _shipmentRepository.GetShipmentGridColumnByName(request.ColumnName, _currentUser.ClientId!);
      if (oExisShipmentGridColumn == null)
      {
        var oShipmentGridClientSettingList = await _shipmentRepository.GetAllShipmentGridClientSetting(_currentUser.ClientId!);
        foreach (var item in oShipmentGridClientSettingList)
        {
          var updatedValues = Utility.RemoveValuesFromString(item.DashboardStatusValue!, request.DashboardStatusIdValues!);
          item.UpdateDashboardStatusValue(updatedValues, _currentUser.EmployeeId);
          //update item with leatest value 
          bool isUpdated = await _shipmentRepository.UpdateShipmentGridClientSetting(item);
        }

        //get display from db on first entry
        List<ShipmentGridColumn> shipmentGrids = await _shipmentRepository.GetAllShipmentGridColumn(_currentUser.ClientId!);

        //get max display order entry
        var getMaxObj = shipmentGrids.OrderByDescending(x => x.DisplayOrder).FirstOrDefault();
        int dispalyOrder = ShipmentGridColumn.GetNextDisplayOrder(getMaxObj);

        ShipmentGridColumn oShipmentGridColumn = ShipmentGridColumn.Create(request.ColumnName!, dispalyOrder, null, null, _currentUser.ClientId!, _currentUser.EmployeeId!);
        bool isCreateShipmentGridColumn = await _shipmentRepository.CreateShipmentGridColumn(oShipmentGridColumn);

        if (isCreateShipmentGridColumn)
        {
          ShipmentGridClientSetting oShipmentGridClientSetting = ShipmentGridClientSetting.Create(oShipmentGridColumn.ShipmentGridColumnId!, request.DashboardStatusIdValues, _currentUser.ClientId!, _currentUser.EmployeeId!);
          bool isCreateoShipmentGridClientSetting = await _shipmentRepository.CreateShipmentGridClientSetting(oShipmentGridClientSetting);
        }
      }
      else
      {
        serviceResult.CreateError("AlreadyExist", new string[] { $"Name with {request.ColumnName} Already exist." });
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
