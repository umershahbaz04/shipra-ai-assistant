using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Core.ProductAggregate;
public partial class ProductStockHistory
{
  public long ProductStockHistorytId { get; set; }
  public int? ReasonId { get; set; }
  public long? ProductStockId { get; set; }
  public int? PreviousQuantity { get; set; }
  public int? NewQuantity { get; set; }
  public DateTime? CreatedOn { get; set; }
  public EmployeeId? CreatedBy { get; set; }
  public string? Comment { get; set; }

  public static ProductStockHistory CreateProductStockHistory(int? reasonId, long? productStockId, int? previousQty, int? newQty, string comment, EmployeeId createdBy)
  {

    return new ProductStockHistory
    {
      ReasonId = reasonId,
      ProductStockId = productStockId,
      PreviousQuantity = previousQty,
      NewQuantity = newQty,
      Comment = comment,
      CreatedBy = createdBy,
      CreatedOn = DateTime.UtcNow
    };
  }
}
