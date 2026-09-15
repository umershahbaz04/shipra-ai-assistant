using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.DriverFeatures.Command.UpdateOrderStatusByDriver;
public class UpdateOrderStatusByDriverCommand : IRequest<ServiceResultDTO>
{
  public int? CarrierTrackingStatusId { get; set; }
  public string? DeliveryNoteDetailId { get; set; }  

}
