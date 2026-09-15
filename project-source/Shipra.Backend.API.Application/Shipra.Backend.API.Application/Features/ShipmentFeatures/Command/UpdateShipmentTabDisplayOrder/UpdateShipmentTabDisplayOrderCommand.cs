using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.SettingOperationDashboardAggregate;

namespace Shipra.Backend.API.Application.Features.ShipmentFeatures.Command.UpdateShipmentTabDisplayOrder;

public class UpdateShipmentTabDisplayOrderCommand : IRequest<ServiceResultDTO>
{
  public List<ShipmentGridColumnModel>? list { get; set; }
}
public class ShipmentGridColumnModel
{
  public int ShipmentGridColumnId { get; set; }
  public int DisplayOrder { get; set; }
}
