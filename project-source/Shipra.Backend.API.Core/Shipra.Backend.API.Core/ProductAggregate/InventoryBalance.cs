using Shipra.Backend.API.SharedKernel;

namespace Shipra.Backend.API.Core.ProductAggregate;

public class InventoryBalance : EntityBase
{
  public InventoryBalance() { }

  public long InventoryBalanceId { get; private set; }
  public long ProductVariantId { get; private set; }
  public int ProductStationId { get; private set; }
  public int QuantityOnHand { get; private set; }
  public int QuantityCommitted { get; private set; }
  public int QuantityIncoming { get; private set; }
  public int QuantityAvailable { get; private set; }
  public int QuantityDamaged { get; private set; }
  public int QuantityOnOrder { get; private set; }
  public int QuantityReserved { get; private set; }
  public DateTime? LastExternalUpdateOn { get; private set; }
  public DateTime CreatedOn { get; private set; }
  public DateTime? UpdatedOn { get; private set; }

  public static InventoryBalance Create(long productVariantId, int productStationId, int quantityAvailable)
  {
      return new InventoryBalance
      {
          ProductVariantId = productVariantId,
          ProductStationId = productStationId,
          QuantityOnHand = quantityAvailable, // As per baseline migration logic
          QuantityAvailable = quantityAvailable,
          QuantityCommitted = 0,
          QuantityIncoming = 0,
          QuantityDamaged = 0,
          QuantityOnOrder = 0,
          QuantityReserved = 0,
          CreatedOn = DateTime.UtcNow
      };
  }

  public void UpdateQuantities(int quantityOnHand, int quantityAvailable, int quantityCommitted)
  {
      QuantityOnHand = quantityOnHand;
      QuantityAvailable = quantityAvailable;
      QuantityCommitted = quantityCommitted;
      UpdatedOn = DateTime.UtcNow;
  }

  public void UpdateFulfillmentStock(int quantityAvailable, int quantityCommitted, int quantityOnOrder)
  {
      QuantityAvailable = quantityAvailable;
      QuantityCommitted = quantityCommitted;
      QuantityOnOrder = quantityOnOrder;
      UpdatedOn = DateTime.UtcNow;
  }

  public void UpdateFulfillmentAndHandStock(int quantityOnHand, int quantityAvailable, int quantityCommitted, int quantityOnOrder)
  {
      QuantityOnHand = quantityOnHand;
      QuantityAvailable = quantityAvailable;
      QuantityCommitted = quantityCommitted;
      QuantityOnOrder = quantityOnOrder;
      UpdatedOn = DateTime.UtcNow;
  }

  public void UpdateDamagedQuantity(int quantityDamaged, int quantityAvailable)
  {
      QuantityDamaged = quantityDamaged;
      QuantityAvailable = quantityAvailable;
      UpdatedOn = DateTime.UtcNow;
  }
}
