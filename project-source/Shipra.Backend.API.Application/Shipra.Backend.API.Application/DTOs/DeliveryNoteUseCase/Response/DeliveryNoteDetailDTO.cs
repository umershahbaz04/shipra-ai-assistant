using Shipra.Backend.API.Application.DTOs.Common.Base.Response;

namespace Shipra.Backend.API.Application.DTOs.DeliveryNoteUseCase.Response;
public class DeliverNoteDetailDTO<T> : ErrorResponse
{
  public T? Result { get; set; }
}
public class DeliveryNoteDetailResponseModel
{
  public string? DeliveryNoteDetailId { get; set; }
  public string? DeliveryNoteId { get; set; }
  public string? OrderId { get; set; }
  public bool? Active { get; set; }
}
