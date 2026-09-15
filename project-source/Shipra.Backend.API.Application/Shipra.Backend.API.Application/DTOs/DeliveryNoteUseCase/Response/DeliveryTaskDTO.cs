using Shipra.Backend.API.Application.DTOs.Common.Base.Response;

namespace Shipra.Backend.API.Application.DTOs.DeliveryNoteUseCase.Response;
public class DeliveryTaskDTO<T> : ErrorResponse
{
  public T? Result { get; set; }
}
public class DeliveryTaskResponseModel
{
  public string? DeliveryTaskId { get; set; }
  public string? JobCode { get; set; }
  public string? OrderId { get; set; }
  public string? DriverId { get; set; }
  public int? DriverPaid { get; set; }
  public DateTime? DriverPaidDate { get; set; }
  public int? DeliveryTaskStatusId { get; set; }
  public string? DriverReceivableId { get; set; }
  public int? LastStatusUpdateId { get; set; }
  public bool? Active { get; set; }
  public int? SortOrder { get; set; }
}
