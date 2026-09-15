using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.SharedKernel;

namespace Shipra.Backend.API.Core.StoresAggregate;

public class SaleChannelVariant : EntityBase
{
  public SaleChannelVariant() { }

  public long SaleChannelVariantId { get; private set; }
  public Guid? SaleChannelProductId { get; private set; }
  public int? SaleChannelLookupId { get; private set; }
  public int? SaleChannelConfigId { get; private set; }
  public int? StoreId { get; private set; }
  public long ProductVariantId { get; private set; }
  public string? ExternalVariantId { get; private set; }
  public string? ExternalSKU { get; private set; }
  public string? ExternalBarcode { get; private set; }
  public string? ExternalCatalogId { get; private set; }
  public string? ExternalCatalogIdType { get; private set; }
  public int? FulfillmentTypeId { get; private set; }
  public string? ProductJson { get; private set; }
  public bool Active { get; private set; }
  public DateTime CreatedOn { get; private set; }
  public EmployeeId? CreatedBy { get; private set; }
  public DateTime? UpdatedOn { get; private set; }
  public EmployeeId? UpdatedBy { get; private set; }
  public DateTime? LastSyncedOn { get; private set; }
}
