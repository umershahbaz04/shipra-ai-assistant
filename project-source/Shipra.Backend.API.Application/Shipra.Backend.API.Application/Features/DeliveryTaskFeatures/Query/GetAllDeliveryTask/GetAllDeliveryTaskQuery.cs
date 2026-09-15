using MediatR;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;

namespace Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Query.GetAllDeliveryTask;
public class GetAllDeliveryTaskQuery : DeliveryTaskFilter, IRequest<ServiceResultDTO>
{ 
}
public class DeliveryTaskFilter : CommonAddressFilterModel
{ 
  public string? StoreIds { get; set; } 
  public string? CarrierTrackingStatusIds { get; set; }
  public string? DriverIds { get; set; }
  public bool? IncludeDriver { get; set; }
  public string? DeliveryTaskStatusIds { get; set; }
  public int? DriverAssignedStatus { get; set; } 
  public string? SalePersonIds { get; set; }
  public string? OrderLabels { get; set; }
  public int? DuplicateStatus { get; set; }
  public int? DeliveryNoteStatusId { get; set; }
}

