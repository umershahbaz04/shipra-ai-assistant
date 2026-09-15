using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.SettingOperationDashboardAggregate;

namespace Shipra.Backend.API.Application.Features.ShipmentFeatures.Command.MoveStatusFromOneTabToAnother;
public class MoveDashboardStatusFromOneTabToAnotherCommand : IRequest<ServiceResultDTO>
{
  public int ShipmentGridColumnId { get; set; }
  public int? CarrierTrackingStatusId { get; set; }
}
public class MoveDashboardStatusFromOneTabToAnotherCommandHandler : RequestHandlerBase<MoveDashboardStatusFromOneTabToAnotherCommand, ServiceResultDTO>
{
  private readonly IShipmentRepository _shipmentRepository;

  public MoveDashboardStatusFromOneTabToAnotherCommandHandler(IShipmentRepository shipmentRepository, IServiceProvider serviceProvider, ILogger<MoveDashboardStatusFromOneTabToAnotherCommandHandler> logger) : base(serviceProvider, logger)
  {
    _shipmentRepository = shipmentRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(MoveDashboardStatusFromOneTabToAnotherCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      //get display from db on first entry
      List<ShipmentGridClientSetting> shipmentGridClientSetting = await _shipmentRepository.GetAllShipmentGridClientSetting(_currentUser.ClientId!);
      foreach (var item in shipmentGridClientSetting)
      {
        var trStatusId = request!.CarrierTrackingStatusId!.ToString()!;
        var list = item.DashboardStatusValue?.Split(",").ToList();
        bool isExist = list!.Contains(trStatusId);
        //check if status already exist in another tab then remove and update string
        if (isExist)
        {
          list.Remove(trStatusId); 
          string result = Utility.ConvertStringListToCsv(list);
          //update item value
          item.UpdateDashboardStatusValue(result, _currentUser.EmployeeId!);
        } 
        //add in selected tab
        if (item.ShipmentGridColumnId == request.ShipmentGridColumnId)
        {
          list.Add(trStatusId);
          string result = Utility.ConvertStringListToCsv(list);
          //update item value
          item.UpdateDashboardStatusValue(result, _currentUser.EmployeeId!);
        } 

        //update item with leatest value 
        bool isUpdated = await _shipmentRepository.UpdateShipmentGridClientSetting(item);

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
