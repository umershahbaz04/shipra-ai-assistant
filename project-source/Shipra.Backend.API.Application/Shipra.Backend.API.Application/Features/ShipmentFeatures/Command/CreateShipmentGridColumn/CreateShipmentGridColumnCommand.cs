using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.ShipmentFeatures.Command.CreateShipmentGridColumn;
public class CreateShipmentGridColumnCommand : IRequest<ServiceResultDTO>
{
  public string? ColumnName { get; set; }
  public string? DashboardStatusIdValues { get; set; }
}
