using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.AccountAggregate;
using Shipra.Backend.API.Core.ActivityLogAggregate;
using Shipra.Backend.API.Core.AppConfigAggregate;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.CarrierReturnReportAggregate;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.DeliveryNoteAggregate;
using Shipra.Backend.API.Core.DeliveryTaskAggregate;
using Shipra.Backend.API.Core.DocumentAggregate;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.ExpenseAggregate;
using Shipra.Backend.API.Core.MenuAggregate;
using Shipra.Backend.API.Core.MetaFieldAggregate;
using Shipra.Backend.API.Core.LeadAggregate;
using Shipra.Backend.API.Core.NotificationAggregate;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.OrderBoxAggregate;
using Shipra.Backend.API.Core.PaymentProcessAggregate;
using Shipra.Backend.API.Core.ProductAggregate;
using Shipra.Backend.API.Core.ProjectAggregate;
using Shipra.Backend.API.Core.ReturnAggregate;
using Shipra.Backend.API.Core.SaleChannelConfigAggregate;
using Shipra.Backend.API.Core.SaleChannelOrderAggregate;
using Shipra.Backend.API.Core.SaleChannelProductAggregate;
using Shipra.Backend.API.Core.SettingOperationDashboardAggregate;
using Shipra.Backend.API.Core.ShopifyAggregate;
using Shipra.Backend.API.Core.SMSProcessAggregate;
using Shipra.Backend.API.Core.StoresAggregate;
using Shipra.Backend.API.Core.TaxAggregate;
using Shipra.Backend.API.Core.UserRoleAndPermissionAggregate;
using Shipra.Backend.API.Core.WalletAggregate;
using Shipra.Backend.API.Core.WebhookEventAggregate;
using Shipra.Backend.API.Core.WhatsappAggregate;
using Shipra.Backend.API.SharedKernel;
using Shipra.Backend.API.SharedKernel.Interfaces;

namespace Shipra.Backend.API.Infrastructure.Data;

public class AppDbContext : DbContext
{
  private readonly IDomainEventDispatcher? _dispatcher; 
  public AppDbContext(DbContextOptions<AppDbContext> options,
    IDomainEventDispatcher? dispatcher)
      : base(options)
  {
    _dispatcher = dispatcher; 
  }
  public DbSet<AppConfig> AppConfigs => Set<AppConfig>();
  public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
  public DbSet<ActiveCarrier> ActiveCarriers => Set<ActiveCarrier>();
  public DbSet<ActiveCarrierPickupLocation> ActiveCarrierPickupLocations => Set<ActiveCarrierPickupLocation>();
  public DbSet<AddressTypeLookup> AddressTypeLookups => Set<AddressTypeLookup>();
  public DbSet<Area> Areas => Set<Area>();
  public DbSet<Carrier> Carriers => Set<Carrier>();
  public DbSet<CarrierFeature> CarrierFeatures => Set<CarrierFeature>();
  public DbSet<BoxTypeLookup> BoxTypeLookups  => Set<BoxTypeLookup>();
  public DbSet<CarrierPaymentSettlement> CarrierPaymentSettlements => Set<CarrierPaymentSettlement>();
  public DbSet<CarrierReturnReport> CarrierReturnReports => Set<CarrierReturnReport>();
  public DbSet<CarrierTrackingStatusLookup> CarrierTrackingStatusLookups => Set<CarrierTrackingStatusLookup>();
  public DbSet<City> Cities => Set<City>();
  public DbSet<Client> Clients => Set<Client>();
  public DbSet<ClientWebhookEvent> ClientWebhookEvents => Set<ClientWebhookEvent>(); 
  public DbSet<ClientTax> ClientTaxes => Set<ClientTax>();
  public DbSet<ClientOrderLabel> ClientOrderLabels  => Set<ClientOrderLabel>();
  public DbSet<ClientOrderLabelLookup> ClientOrderLabelLookups => Set<ClientOrderLabelLookup>();
  public DbSet<ClientAddress> ClientAddresses => Set<ClientAddress>();
  public DbSet<ClientCarrierTrackingStatus> ClientCarrierTrackingStatuses => Set<ClientCarrierTrackingStatus>();
  public DbSet<ClientUserRole> ClientUserRolees => Set<ClientUserRole>();
  public DbSet<ClientRolePermissionGroup> ClientRolePermissionGroups => Set<ClientRolePermissionGroup>();
  public DbSet<ClientOrderBox> ClientOrderBoxes => Set<ClientOrderBox>();
  public DbSet<ClientPayoutBank> ClientPayoutBanks => Set<ClientPayoutBank>();
  public DbSet<Country> Countries => Set<Country>();
  public DbSet<ShipraContractClientCarrier> ShipraContractClientCarriers => Set<ShipraContractClientCarrier>();
  public DbSet<Province> Provinces => Set<Province>();
  public DbSet<CarrierLocation> CarrierLocations => Set<CarrierLocation>();
  public DbSet<CPSettlementPopFile> CpsettlementPopFiles => Set<CPSettlementPopFile>();
  public DbSet<CarrierDeliveryService> CarrierDeliveryServices => Set<CarrierDeliveryService>();
  public DbSet<DeliveryNote> DeliveryNotes => Set<DeliveryNote>();
  public DbSet<DeliveryService> DeliveryServices => Set<DeliveryService>();
  public DbSet<DeliveryNoteDetail> DeliveryNoteDetails => Set<DeliveryNoteDetail>();
  public DbSet<DeliveryTask> DeliveryTasks => Set<DeliveryTask>();
  public DbSet<DeliveryTaskStatusLookup> DeliveryTaskStatusLookups => Set<DeliveryTaskStatusLookup>();
  public DbSet<Driver> Drivers => Set<Driver>();
  public DbSet<DocumentSize> DocumentSizes => Set<DocumentSize>();
  public DbSet<DocumentTemplate> DocumentTemplates => Set<DocumentTemplate>();
  public DbSet<DocumentTemplateConfig> DocumentTemplateConfigs => Set<DocumentTemplateConfig>();
  public DbSet<DocumentType> DocumentTypes => Set<DocumentType>();

  public DbSet<Expense> Expenses => Set<Expense>();
  public DbSet<ExpenseCategory> ExpenseCategories => Set<ExpenseCategory>();
  public DbSet<DriverReceivable> DriverReceivables => Set<DriverReceivable>();
  public DbSet<DriverCTSSetting> DriverCTSSettings => Set<DriverCTSSetting>();
  public DbSet<Employee> Employees => Set<Employee>();
  public DbSet<EmployeeType> EmployeeTypes => Set<EmployeeType>();
  public DbSet<EmployeeAddress> EmployeeAddresses => Set<EmployeeAddress>();
  public DbSet<EmployeeColumnConfiguration> EmployeeColumnConfigurations => Set<EmployeeColumnConfiguration>();
  public DbSet<GenderLookup> GenderLookups => Set<GenderLookup>();
  public DbSet<ExpenseCategoryLookup> ExpenseCategoryLookups => Set<ExpenseCategoryLookup>();

  public DbSet<FullFillmentStatusLookup> FullFillmentStatusLookups => Set<FullFillmentStatusLookup>();
  public DbSet<ImageGallery> ImageGalleries => Set<ImageGallery>();
  public DbSet<LookupAdjustReason> LookupAdjustReasons => Set<LookupAdjustReason>();
  public DbSet<LookupProductStockStatus> LookupProductStockStatuses => Set<LookupProductStockStatus>();

  public DbSet<Mcconfig> Mcconfigs => Set<Mcconfig>();
  public DbSet<Menu> Menus => Set<Menu>();
  // public DbSet<LanguageConfig> LanguageConfigs => Set<LanguageConfig>();
  public DbSet<Lead> Leads => Set<Lead>();
  public DbSet<LeadStatusLookup> LeadStatusLookups => Set<LeadStatusLookup>();
  public DbSet<ClientLeadStatusLookup> ClientLeadStatusLookups => Set<ClientLeadStatusLookup>();
  public DbSet<LeadGridColumn> LeadGridColumns => Set<LeadGridColumn>();
  public DbSet<LeadGridClientSetting> LeadGridClientSettings => Set<LeadGridClientSetting>();
  // public DbSet<MapKey> MapKeys => Set<MapKey>();
  public DbSet<MenuItem> MenuItems => Set<MenuItem>();
  public DbSet<MenuGroupItem> MenuGroupItems => Set<MenuGroupItem>();
  public DbSet<MenuOtherRoute> MenuOtherRoutes => Set<MenuOtherRoute>(); 
  public DbSet<NotificationType> NotificationTypes => Set<NotificationType>(); 
  public DbSet<NotificationChannel> NotificationChannels => Set<NotificationChannel>(); 
  public DbSet<NotificationConfig> NotificationConfigs => Set<NotificationConfig>(); 
  public DbSet<NotificationEvent> NotificationEvents => Set<NotificationEvent>(); 

  public DbSet<ONGFTypeLookup> ONGFTypeLookups => Set<ONGFTypeLookup>();
  public DbSet<OrderAddress> OrderAddresses => Set<OrderAddress>();
  public DbSet<Order> Orders => Set<Order>();
  public DbSet<OrderDeleted> OrderDeleteds => Set<OrderDeleted>();
  public DbSet<OrderTax> OrderTaxes => Set<OrderTax>();
  public DbSet<OrderItem> OrderItems => Set<OrderItem>();
  public DbSet<OrderNote> OrderNotes => Set<OrderNote>();
  public DbSet<OrderTrackingHistory> OrderTrackingHistories => Set<OrderTrackingHistory>();
  public DbSet<OrderPODFile> OrderPODFiles => Set<OrderPODFile>();
  public DbSet<OrderUplaodSampleFile> OrderUplaodSampleFiles => Set<OrderUplaodSampleFile>();
  public DbSet<StoreUploadSampleFile> StoreUplaodSampleFiles => Set<StoreUploadSampleFile>();
  public DbSet<OrderBox> OrderBoxes => Set<OrderBox>();
  public DbSet<PaymentStatusLookup> PaymentStatusLookups => Set<PaymentStatusLookup>();
  public DbSet<ProductCategoryLookup> ProductCategoryLookups => Set<ProductCategoryLookup>();
  public DbSet<Product> Products => Set<Product>();
  public DbSet<ProductOption> ProductOptions => Set<ProductOption>();
  public DbSet<ProductOptionLookup> ProductOptionLookups => Set<ProductOptionLookup>();
  public DbSet<ProductStation> ProductStations => Set<ProductStation>();
  public DbSet<ProductMedia> ProductMedias => Set<ProductMedia>();
  public DbSet<ProductLinkToken> ProductLinkTokens => Set<ProductLinkToken>();
  public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
  public DbSet<ProductVariantOption> ProductVariantOptions => Set<ProductVariantOption>();
  public DbSet<InventoryBalance> InventoryBalances => Set<InventoryBalance>();
  public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();
  public DbSet<SaleChannelVariant> SaleChannelVariants => Set<SaleChannelVariant>();
  public DbSet<ProductStationTypeLookup> ProductStationTypeLookups => Set<ProductStationTypeLookup>();
  public DbSet<FulfillmentTypeLookup> FulfillmentTypeLookups => Set<FulfillmentTypeLookup>();
  public DbSet<ProductStationTransfer> ProductStationTransfers => Set<ProductStationTransfer>();
  public DbSet<PermissionAction> PermissionActions => Set<PermissionAction>();
  public DbSet<PermissionGroupLookup> PermissionGroupLookups => Set<PermissionGroupLookup>();
  public DbSet<RolePermissionGroupDefault> RolePermissionGroupDefaults => Set<RolePermissionGroupDefault>();
  public DbSet<MenuItemPermissionClient> MenuItemPermissionClients => Set<MenuItemPermissionClient>();
  //public DbSet<Region> Regions => Set<Region>();
  public DbSet<RegionTimeZone> RegionTimeZones => Set<RegionTimeZone>();
  public DbSet<TaxTypeLookup> TaxTypeLookups => Set<TaxTypeLookup>();
  public DbSet<TransferProduct> TransferProducts => Set<TransferProduct>();
  public DbSet<ProductStock> ProductStocks => Set<ProductStock>();
  public DbSet<ProductStockHistory> ProductStockHistories => Set<ProductStockHistory>();
  public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
  public DbSet<Ppactivate> Ppactivates => Set<Ppactivate>();
  public DbSet<Pplookup> Pplookups => Set<Pplookup>();

  public  DbSet<PaymentLink> PaymentLinks => Set<PaymentLink>(); 
  public DbSet<PaymentLinkStatusLookup> PaymentLinkStatusLookups => Set<PaymentLinkStatusLookup>();
  public DbSet<Wallet> Wallets => Set<Wallet>();
  public DbSet<Payout> Payouts => Set<Payout>();
  public DbSet<PayoutFile> PayoutFiles => Set<PayoutFile>(); 
  public DbSet<PayoutStatusHistory> PayoutStatusHistories => Set<PayoutStatusHistory>(); 
  public DbSet<PayoutStatusLookup> PayoutStatusLookups => Set<PayoutStatusLookup>();
  public DbSet<Transaction> Transactions => Set<Transaction>(); 
  public DbSet<TransactionTypeLookup> TransactionTypeLookups => Set<TransactionTypeLookup>();
  public DbSet<SaleChannelConfig> SaleChannelConfigs => Set<SaleChannelConfig>();
  public DbSet<SaleChannelLookup> SaleChannelLookups => Set<SaleChannelLookup>();
  public DbSet<SaleChannelOrder> SaleChannelOrders => Set<SaleChannelOrder>();
  public DbSet<SaleChannelProduct> SaleChannelProducts => Set<SaleChannelProduct>();
  public DbSet<ShopifyConfig> ShopifyConfigs => Set<ShopifyConfig>();
  public DbSet<ScfolderLookup> ScfolderLookups => Set<ScfolderLookup>();
  public DbSet<SMSActivate> SMSActivates => Set<SMSActivate>();
  public DbSet<StoreProduct> StoreProducts => Set<StoreProduct>();
  public DbSet<SMSLookup> SMSlookups => Set<SMSLookup>();
  public DbSet<DefaultShipmentDashboard> DefaultShipmentDashboards => Set<DefaultShipmentDashboard>();
  public DbSet<ShipmentGridColumn> ShipmentGridColumns => Set<ShipmentGridColumn>();
  public DbSet<ShipmentGridClientSetting> ShipmentGridClientSettings => Set<ShipmentGridClientSetting>();
  public DbSet<Store> Stores => Set<Store>();
  public DbSet<StoreAddress> StoreAddresses => Set<StoreAddress>();
  public DbSet<StationLookup> StationLookups => Set<StationLookup>();
  public DbSet<PaymentMethodLookup> PaymentMethodLookups => Set<PaymentMethodLookup>();
  public DbSet<OrderTypeLookup> OrderTypeLookups => Set<OrderTypeLookup>();
  public DbSet<ToDoItem> ToDoItems => Set<ToDoItem>();
  public DbSet<UserRoleLookup> UserRoleLookups => Set<UserRoleLookup>(); 
  public DbSet<WhatsAppCategoryType> WhatsAppCategoryTypeLookups => Set<WhatsAppCategoryType>();
  public DbSet<Zone> Zones => Set<Zone>();
  public DbSet<ClientReturnReason> ClientReturnReasons => Set<ClientReturnReason>();
  public DbSet<ClientGenericSetting> ClientGenericSettings => Set<ClientGenericSetting>();
  public DbSet<ClientGenericSettingLookup> ClientGenericSettingLookups => Set<ClientGenericSettingLookup>();
  public DbSet<ClientConfigSetting> ClientConfigSettings => Set<ClientConfigSetting>();
  public DbSet<Return> Returns => Set<Return>();
  public DbSet<RefundTypeLookup> RefundTypeLookups => Set<RefundTypeLookup>();
  public DbSet<ReturnActivityLog> ReturnActivityLogs => Set<ReturnActivityLog>();
  public DbSet<ReturnTrackingHistory> ReturnTrackingHistories => Set<ReturnTrackingHistory>();
  public DbSet<ReturnProduct> ReturnProducts => Set<ReturnProduct>();
  public DbSet<ReturnStatusLookup> ReturnStatusLookups => Set<ReturnStatusLookup>();
  public DbSet<WebhookEventLookup> WebhookEventLookups => Set<WebhookEventLookup>();
  public DbSet<WebhookEventLog> WebhookEventLogs  => Set<WebhookEventLog>();
  public DbSet<WhatsappActivate> WhatsappActivates => Set<WhatsappActivate>();
  public DbSet<WhatsappLookup> WhatsappLookups => Set<WhatsappLookup>();
  public DbSet<EntityMetaFieldLookup> EntityMetaFieldLookups => Set<EntityMetaFieldLookup>();
  public DbSet<ClientMetaField> ClientMetaFields => Set<ClientMetaField>();
  public DbSet<MetaField> MetaFields => Set<MetaField>();
  public DbSet<OrderDraft> OrderDrafts => Set<OrderDraft>();
  public DbSet<OrderArchive> OrderArchives => Set<OrderArchive>();


  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    //modelBuilder.ApplyConfiguration(new ProductConfiguration());

    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder.EnableSensitiveDataLogging();
  }
  public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
  {
    int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

    // ignore events if no dispatcher provided
    if (_dispatcher == null) return result;

    // dispatch events only if save was successful
    var entitiesWithEvents = ChangeTracker.Entries<EntityBase>()
        .Select(e => e.Entity)
        .Where(e => e.DomainEvents.Any())
        .ToArray();

    await _dispatcher.DispatchAndClearEvents(entitiesWithEvents);

    return result;
  }

  public override int SaveChanges()
  {
    return SaveChangesAsync().GetAwaiter().GetResult();
  }
}
