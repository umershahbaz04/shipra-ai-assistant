using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Application.DTOs.ShipmentUseCase;
public class UpdateShipmentGridColumnRequestModel
{
  public int shipmentGridColumnId { get; set; }
  public string? columnName { get; set; }
  public string? dashboardStatusValue { get; set; }
}
