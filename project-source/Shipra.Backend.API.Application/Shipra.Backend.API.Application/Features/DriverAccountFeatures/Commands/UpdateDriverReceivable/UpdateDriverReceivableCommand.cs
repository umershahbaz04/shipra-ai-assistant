using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Application.Features.DriverAccountFeatures.Commands.UpdateDriverReceivable;
public class UpdateDriverReceivableCommand : IRequest<ServiceResultDTOWithTypeModel<BaseResponseDto>>
{
  public string? DriverReceivableId { get; set; }

  public string? DriverId { get; set; }
  public DateTime? ReceiveDate { get; set; }
  public decimal? Expense { get; set; }
  public decimal? Cash { get; set; }
  public decimal? Total { get; set; } 
  public bool? Active { get; set; } 
  public string? DeliveryNoteId { get; set; }
}
