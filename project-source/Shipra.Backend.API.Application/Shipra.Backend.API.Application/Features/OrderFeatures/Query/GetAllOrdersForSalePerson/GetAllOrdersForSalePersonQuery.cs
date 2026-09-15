using MediatR;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetAllOrdersForSalePerson;
public class GetAllOrdersForSalePersonQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
  public string? StoreId { get; set; }
  public int? OrderTypeId { get; set; }
  public string? CarrierId { get; set; }
  public int? FullFillmentStatusId { get; set; }
  public int? PaymentStatusId { get; set; }
  public int? OrderRequestVia { get; set; }
  public int? PaymentMethodId { get; set; }
  public string? StationId { get; set; }
  public bool ReadyForAssignment { get; set; } = false;
  public bool Assigned { get; set; } = false;
}
