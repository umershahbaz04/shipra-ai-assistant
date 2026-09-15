using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.SharedKernel;

namespace Shipra.Backend.API.Core.ProductAggregate;

public class InventoryTransaction : EntityBase
{
  public InventoryTransaction() { }

  public long InventoryTransactionId { get; private set; }
  public long ProductVariantId { get; private set; }
  public int ProductStationId { get; private set; }
  public long? WarehouseLocationId { get; private set; }
  public int TransactionTypeId { get; private set; }
  public decimal Quantity { get; private set; }
  public long? FromLocationId { get; private set; }
  public long? ToLocationId { get; private set; }
  public int? ReferenceTypeId { get; private set; }
  public string? ReferenceId { get; private set; }
  public decimal? PreviousQuantity { get; private set; }
  public decimal? NewQuantity { get; private set; }
  public string? Comment { get; private set; }
  public DateTime CreatedOn { get; private set; }
  public EmployeeId? CreatedBy { get; private set; }

  public static InventoryTransaction Create(
      long productVariantId, 
      int productStationId, 
      int transactionTypeId, 
      decimal quantity, 
      decimal? previousQuantity, 
      decimal? newQuantity, 
      string? comment, 
      EmployeeId? createdBy)
  {
      return new InventoryTransaction
      {
          ProductVariantId = productVariantId,
          ProductStationId = productStationId,
          TransactionTypeId = transactionTypeId,
          Quantity = quantity,
          PreviousQuantity = previousQuantity,
          NewQuantity = newQuantity,
          Comment = comment,
          CreatedOn = DateTime.UtcNow,
          CreatedBy = createdBy
      };
  }
}
