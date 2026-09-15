using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.DriverAccountFeatures.Commands.CreateDriverReceivable;
public class CreateDriverReceivableCommand : IRequest<ServiceResultDTO>
{ 
  public string? DriverId { get; set; }
  public DateTime? ReceiveDate { get; set; }
  public decimal? Expense { get; set; }
  public decimal? Cash { get; set; }
  public decimal? Total { get; set; } 
  public bool? Active { get; set; }
  public string? DeliveryNoteId { get; set; }
}
