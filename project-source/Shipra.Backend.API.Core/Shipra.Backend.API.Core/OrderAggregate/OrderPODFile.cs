using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Core.OrderAggregate;
public class OrderPODFile
{
  public OrderPODFileId? OrderPodfileId { get; set; }
  public OrderId? OrderId { get; set; }
  public string? Comment { get; set; }
  public string? FilePath { get; set; }
  public string? Extension { get; set; }
  public bool? Active { get; set; }
  public DateTime? CreatedOn { get; set; }
  public EmployeeId? CreatedBy { get; set; }
  public DateTime? UpdatedOn { get; set; }
  public EmployeeId? UpdatedBy { get; set; }
  public static OrderPODFile CreateOrderPODFiles(OrderId orderId, string comment, string filePath,string? extension, EmployeeId createdBy)
  {
    return new OrderPODFile
    {
      OrderPodfileId = OrderPODFileId.New,
      OrderId = orderId,
      Comment = comment,
      FilePath = filePath,
      Extension = extension,
      Active = true,
      CreatedOn = DateTime.UtcNow,
      CreatedBy = createdBy
    };
  }
}
public sealed record OrderPODFileId(Guid Value)
{
  public static OrderPODFileId New => new(Guid.NewGuid());
}
