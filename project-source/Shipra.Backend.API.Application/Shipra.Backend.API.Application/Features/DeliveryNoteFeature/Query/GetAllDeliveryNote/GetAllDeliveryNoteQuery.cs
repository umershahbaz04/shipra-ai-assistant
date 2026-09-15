using MediatR;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetAllDeliveryNote;
public class GetAllDeliveryNoteQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
  public int DeliveryNoteStatusId { get; set; }
  public string? DriverIds { get; set; }
}
