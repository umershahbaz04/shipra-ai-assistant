using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.ReturnReportFeature.Query.GetOrderDetailForReturnReport;
public class GetOrderDetailForReturnReportQuery : IRequest<ServiceResultDTO>
{
  public string? OrderNo { get; set; }
  public string? TrackingNo { get; set; }
  public int CarrierId { get; set; }

}
