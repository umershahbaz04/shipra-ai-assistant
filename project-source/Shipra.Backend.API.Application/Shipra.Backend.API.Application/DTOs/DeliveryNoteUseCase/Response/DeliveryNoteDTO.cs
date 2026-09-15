using Shipra.Backend.API.Application.DTOs.Common.Base.Response;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Application.DTOs.DeliveryNoteUseCase.Response;
public class DeliverNoteDTO<T> : ErrorResponse
{
  public T? Result { get; set; }
}
public class DeliveryNoteResponseModel
{
  public string? DeliveryNoteId { get; set; }
  public string? NoteNo { get; set; }
  public string? DriverId { get; set; }
  public int? ShipmentCount { get; set; } 
  public DateTime? CreatedOn { get; set; } 
  public DateTime? UpdateOn { get; set; }
  public bool? Active { get; set; }
}
