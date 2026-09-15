using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;

namespace Shipra.Backend.API.Core.OrderAggregate;
public class OrderTrackingHistory
{
  public OrderTrackingHistoryId? OrderTrackingHistoryId { get; set; }
  public OrderId? OrderId { get; set; }
  public int? CarrierTrackingStatusId { get; set; }
  public int? OrderHistoryTypeId { get; private set; }
  public bool? Active { get; set; }
  public string? TrackingStatusComments { get; set; }
  public string? CreatedByName { get; set; }
  public string? Location { get; private set; }
  public decimal? Latitude { get; private set; }
  public decimal? Longitude { get; private set; }
  public EmployeeId? CreatedBy { get; set; }
  public DateTime? CreatedOn { get; set; }
  public static OrderTrackingHistory CreateOrderTrackingHistory(OrderId orderId, int? trackingStatusId, string? comment, EmployeeId createdBy,string? createdByName,int enumOrderHistoryTypeid = (int)EnumOrderHistoryType.ShipraApp)
  {
    return new OrderTrackingHistory()
    {
      OrderTrackingHistoryId = OrderTrackingHistoryId.New,
      OrderId = orderId,
      CarrierTrackingStatusId = trackingStatusId,
      OrderHistoryTypeId = enumOrderHistoryTypeid,
      CreatedByName = createdByName,
      Active = true,
      TrackingStatusComments = comment,
      CreatedBy = createdBy,
      CreatedOn = DateTime.UtcNow
    };
  }
}
public sealed record OrderTrackingHistoryId(Guid Value)
{
  public static OrderTrackingHistoryId New => new(Guid.NewGuid());
}
