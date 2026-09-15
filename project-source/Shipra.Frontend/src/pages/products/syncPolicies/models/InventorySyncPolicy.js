/**
 * Enum definitions for Inventory Sync Policy
 * Aligned with SQL Server table: [dbo].[InventorySyncPolicy]
 */

export const EnumInventorySyncMode = {
  AllProducts: 1,
  SelectedProducts: 2,
  properties: {
    1: { name: "All products", value: 1, description: "Use this as the default policy for every mapped product on the selected channel." },
    2: { name: "Selected products", value: 2, description: "Apply only to specific products, collections, brands or categories." },
  },
};

export const EnumInventorySyncDirection = {
  ShipraToChannel: 1,
  ChannelToShipra: 2,
  TwoWay: 3,
  properties: {
    1: { name: "Shipra to Channel (One-way)", value: 1 },
    2: { name: "Channel to Shipra (One-way)", value: 2 },
    3: { name: "Two-way Sync", value: 3 },
  },
};

export const EnumStationSourceMode = {
  AllStations: 1,
  SelectedStations: 2,
  properties: {
    1: { name: "All Product Stations", value: 1, description: "Combine available stock from every active inventory location." },
    2: { name: "Selected Product Stations", value: 2, description: "Choose exactly which warehouses or stores can contribute stock." },
  },
};

export const defaultFormState = {
  InventorySyncPolicyId: 0,
  policyName: "",
  description: "",
  saleChannelConfigId: null,
  salesChannelName: "",
  salesChannelCode: "",
  inventorySyncModeId: 3,
  stationSourceModeId: EnumStationSourceMode.AllStations,
  stations: [],
  products: [],
  selectedProductIds: [],
  bufferQuantity: 5,
  minPublishQuantity: 0,
  maxPublishQuantity: 50,
  allocationPercentage: 5,
  includeIncoming: false,
  includeCommitted: false,
  includeDamaged: false,
  priority: 1,
  active: true,
  syncInventory: true,
  setUnavailableToZero: true,
  syncPrice: false,
  syncProductInfo: false,
  syncMedia: false,
};

/**
 * InventorySyncPolicy Aggregate Model
 * Maps directly to backend API & [dbo].[InventorySyncPolicy] table schema
 */
export class InventorySyncPolicy {
  constructor(data = {}) {
    this.InventorySyncPolicyId = data.InventorySyncPolicyId || 0;
    this.PolicyName = data.PolicyName || data.name || "";
    this.Description = data.Description || data.description || "";
    this.ClientId = data.ClientId || "00000000-0000-0000-0000-000000000000";
    this.StoreId = data.StoreId || null;
    this.SaleChannelConfigId = data.SaleChannelConfigId || 1;
    this.SaleChannelLookupId = data.SaleChannelLookupId || null;
    this.ChannelLogoUrl = data.ChannelLogoUrl || data.channelLogoUrl || data.ImageUrl || data.imageUrl || "";
    this.SalesChannelName = data.SalesChannelName || data.SaleChannelName || "Amazon UAE";
    this.SalesChannelCode = data.SalesChannelCode || (this.SalesChannelName ? this.SalesChannelName[0]?.toUpperCase() : "S");
    this.ProductId = data.ProductId || null;
    this.ProductVariantId = data.ProductVariantId || null;
    
    // Core Sync Modes
    this.InventorySyncModeId = data.InventorySyncModeId || EnumInventorySyncMode.AllProducts;
    this.InventorySyncDirectionId = data.InventorySyncDirectionId || EnumInventorySyncDirection.ShipraToChannel;
    this.StationSourceModeId = data.StationSourceModeId || EnumStationSourceMode.SelectedStations;

    // Summaries from Backend DTO
    this.StationSummary = data.StationSummary || "All Stations";
    this.ProductsScopeSummary = data.ProductsScopeSummary || (this.InventorySyncModeId === EnumInventorySyncMode.AllProducts ? "All products" : "Selected products");
    this.InventoryRuleSummary = data.InventoryRuleSummary || "";

    // Rules & Thresholds
    this.BufferQuantity = data.BufferQuantity !== undefined ? Number(data.BufferQuantity) : 5;
    this.MaxPublishQuantity = data.MaxPublishQuantity !== undefined && data.MaxPublishQuantity !== null ? Number(data.MaxPublishQuantity) : 50;
    this.MinPublishQuantity = data.MinPublishQuantity !== undefined && data.MinPublishQuantity !== null ? Number(data.MinPublishQuantity) : 0;
    this.AllocationPercentage = data.AllocationPercentage !== undefined ? Number(data.AllocationPercentage) : 100.0;
    
    // Inventory Inclusions
    this.IncludeIncoming = Boolean(data.IncludeIncoming);
    this.IncludeCommitted = Boolean(data.IncludeCommitted);
    this.IncludeDamaged = Boolean(data.IncludeDamaged);
    this.Priority = data.Priority !== undefined ? Number(data.Priority) : 1;
    this.Active = data.Active !== undefined ? Boolean(data.Active) : true;

    // Sync Toggles
    this.SyncInventory = data.SyncInventory !== undefined ? Boolean(data.SyncInventory) : true;
    this.SetUnavailableToZero = data.SetUnavailableToZero !== undefined ? Boolean(data.SetUnavailableToZero) : true;
    this.SyncPrice = Boolean(data.SyncPrice);
    this.SyncProductInfo = Boolean(data.SyncProductInfo);
    this.SyncMedia = Boolean(data.SyncMedia);

    // Associated Entities / Mappings
    this.ProductStationMappings = data.ProductStationMappings || [];
    this.SelectedProductIds = data.SelectedProductIds || [];
    this.SelectedProductsCount = data.SelectedProductsCount || 0;

    // Audit Fields
    this.CreatedOn = data.CreatedOn || new Date().toISOString();
    this.RawCreatedOn = data.RawCreatedOn || data.CreatedOn || null;
    this.CreatedBy = data.CreatedBy || null;
    this.UpdatedOn = data.UpdatedOn || null;
    this.RawUpdatedOn = data.RawUpdatedOn || data.UpdatedOn || null;
    this.UpdatedBy = data.UpdatedBy || null;
    this.LastSync = data.LastSync || "";
  }

  /**
   * Convert UI Form State to Backend API Payload matching [dbo].[InventorySyncPolicy]
   */
  static toSavePayload(formState) {
    const selectedStations = (formState.stations || []).filter((s) => s.Selected);
    
    return {
      InventorySyncPolicyId: formState.InventorySyncPolicyId || 0,
      PolicyName: formState.policyName || "",
      Description: formState.description || "",
      ClientId: formState.clientId || "00000000-0000-0000-0000-000000000000",
      StoreId: formState.storeId || null,
      SaleChannelConfigId: formState.saleChannelConfigId || 1,
      SalesChannelName: formState.salesChannelName || "Amazon UAE",
      ProductId: formState.productId || null,
      ProductVariantId: formState.productVariantId || null,
      InventorySyncModeId: formState.inventorySyncModeId || EnumInventorySyncMode.AllProducts,
      InventorySyncDirectionId: formState.inventorySyncDirectionId || EnumInventorySyncDirection.ShipraToChannel,
      BufferQuantity: Number(formState.bufferQuantity || 0),
      MaxPublishQuantity: formState.maxPublishQuantity !== "" && formState.maxPublishQuantity !== null ? Number(formState.maxPublishQuantity) : null,
      MinPublishQuantity: Number(formState.minPublishQuantity || 0),
      AllocationPercentage: Number(formState.allocationPercentage || 100),
      IncludeIncoming: Boolean(formState.includeIncoming),
      IncludeCommitted: Boolean(formState.includeCommitted),
      IncludeDamaged: Boolean(formState.includeDamaged),
      Priority: Number(formState.priority || 1),
      Active: Boolean(formState.active !== undefined ? formState.active : true),
      ProductStationIds: selectedStations.map((s) => s.ProductStationId),
      ProductVariantIds: (formState.products || []).filter((p) => p.Selected).map((p) => p.ProductVariantId),
    };
  }
}
