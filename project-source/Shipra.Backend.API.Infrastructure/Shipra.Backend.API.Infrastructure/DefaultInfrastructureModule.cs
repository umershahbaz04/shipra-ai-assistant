using System.Reflection;
using Autofac;
using MediatR;
using MediatR.Pipeline;
using Shipra.Backend.API.Application.Services.Implementation.Modified;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProjectAggregate;
using Shipra.Backend.API.Core.Services;
using Shipra.Backend.API.Infrastructure.Data;
using Shipra.Backend.API.Infrastructure.Data.Repository;
using Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
using Shipra.Backend.API.Infrastructure.Factories;
using Shipra.Backend.API.Infrastructure.Services;
using Shipra.Backend.API.Infrastructure.Services.Implementation;
using Shipra.Backend.API.Infrastructure.Services.Interface;
using Shipra.Backend.API.Infrastructure.Services.Permissions;
using Shipra.Backend.API.Security.Service.IManagers;
using Shipra.Backend.API.Security.Service.Managers;
using Shipra.Backend.API.SharedKernel;
using Shipra.Backend.API.SharedKernel.Interfaces;
using Shipra.Backend.API.SharedKernel.Repository;
using Module = Autofac.Module;

namespace Shipra.Backend.API.Infrastructure;

public class DefaultInfrastructureModule : Module
{
  private readonly bool _isDevelopment = false;
  private readonly List<Assembly> _assemblies = new List<Assembly>();

  public DefaultInfrastructureModule(bool isDevelopment, Assembly? callingAssembly = null)
  {
    _isDevelopment = isDevelopment;
    var coreAssembly =
      Assembly.GetAssembly(typeof(Project)); // TODO: Replace "Project" with any type from your Core project
    var infrastructureAssembly = Assembly.GetAssembly(typeof(StartupSetup));
    if (coreAssembly != null)
    {
      _assemblies.Add(coreAssembly);
    }

    if (infrastructureAssembly != null)
    {
      _assemblies.Add(infrastructureAssembly);
    }

    if (callingAssembly != null)
    {
      _assemblies.Add(callingAssembly);
    }
  }

  protected override void Load(ContainerBuilder builder)
  {
    if (_isDevelopment)
    {
      RegisterDevelopmentOnlyDependencies(builder);
    }
    else
    {
      RegisterProductionOnlyDependencies(builder);
    }

    RegisterCommonDependencies(builder);
  }

  private void RegisterCommonDependencies(ContainerBuilder builder)
  {
    builder.RegisterGeneric(typeof(EfRepository<>))
      .As(typeof(IRepository<>))
      .As(typeof(IReadRepository<>))
      .InstancePerDependency();

    builder
      .RegisterType<Mediator>()
      .As<IMediator>()
      .InstancePerDependency();

    builder
      .RegisterType<DomainEventDispatcher>()
      .As<IDomainEventDispatcher>()
      .InstancePerDependency();

    builder.Register<ServiceFactory>(context =>
    {
      var c = context.Resolve<IComponentContext>();

      return t => c.Resolve(t);
    });

    var mediatrOpenTypes = new[]
    {
      typeof(IRequestHandler<,>),
      typeof(IRequestExceptionHandler<,,>),
      typeof(IRequestExceptionAction<,>),
      typeof(INotificationHandler<>),
    };

    foreach (var mediatrOpenType in mediatrOpenTypes)
    {
      builder
        .RegisterAssemblyTypes(_assemblies.ToArray())
        .AsClosedTypesOf(mediatrOpenType)
        .AsImplementedInterfaces();
    }
  }

  private void RegisterDevelopmentOnlyDependencies(ContainerBuilder builder)
  {
    RegisterDependencyForBothEnvironment(builder);
  }
  private void RegisterDependencyForBothEnvironment(ContainerBuilder builder)
  {
    // NOTE: Add any development only services here
    builder.RegisterType<FakeEmailSender>().As<IEmailSender>().InstancePerDependency();
    builder.RegisterType<ProductRepository>().As<IProductRepository>().InstancePerDependency();
    builder.RegisterType<InventoryRepository>().As<IInventoryRepository>().InstancePerDependency();
    builder.RegisterType<DashboardRepository>().As<IDashboardRepository>().InstancePerDependency();
    builder.RegisterType<DriverRepository>().As<IDriverRepository>().InstancePerDependency();
    builder.RegisterType<ProductStationRepository>().As<IProductStationRepository>().InstancePerDependency();
    builder.RegisterType<ProductStationTransferRepository>().As<IProductStationTransferRepository>().InstancePerDependency();
    builder.RegisterType<ProductCategoryRepository>().As<IProductCategoryRepository>().InstancePerDependency();
    builder.RegisterType<ClientRepository>().As<IClientRepository>().InstancePerDependency();
    builder.RegisterType<ClientLoginRepository>().As<IClientLoginRepository>().InstancePerDependency();
    builder.RegisterType<CountryRepository>().As<ICountryRepository>().InstancePerDependency();
    builder.RegisterType<CommonLookupRepository>().As<ICommonLookupRepository>().InstancePerDependency();
    builder.RegisterType<OrderRepository>().As<IOrderRepository>().InstancePerDependency();
    builder.RegisterType<UserManagementRepository>().As<IUserManagement>().InstancePerDependency();
    builder.RegisterType<DeliveryTaskRepository>().As<IDeliveryTaskRepository>().InstancePerDependency();
    builder.RegisterType<LeadRepository>().As<ILeadRepository>().InstancePerDependency();
    builder.RegisterType<DeliveryNoteRepository>().As<IDeliveryNoteRepository>().InstancePerDependency();
    builder.RegisterType<DriverAccountRepository>().As<IDriverAccountRepository>().InstancePerDependency();
    builder.RegisterType<CarrierRepository>().As<ICarrierRepository>().InstancePerDependency();
    builder.RegisterType<OrderTrackingHistoryRepository>().As<IOrderTrackingHistoryRepository>().InstancePerDependency();
    builder.RegisterType<StationLookupRepository>().As<IStationLookupRepository>().InstancePerDependency();
    builder.RegisterType<PaymentMethodLookupRepository>().As<IPaymentMethodLookupRepository>().InstancePerDependency();
    builder.RegisterType<OrderTypeLookupRepository>().As<IOrderTypeLookupRepository>().InstancePerDependency();
    builder.RegisterType<ShipmentRepository>().As<IShipmentRepository>().InstancePerDependency();
    builder.RegisterType<ShipmentStatusCommonRepository>().As<IShipmentStatusCommonRepository>().InstancePerDependency();
    builder.RegisterType<SaleChannelOrderRepository>().As<ISaleChannelOrderRepository>().InstancePerDependency();
    builder.RegisterType<SaleChannelProductRepository>().As<ISaleChannelProductRepository>().InstancePerDependency();
    builder.RegisterType<ConfigRepository>().As<IConfigRepository>().InstancePerDependency();
    builder.RegisterType<S3Service>().As<IS3Service>().InstancePerDependency();
    builder.RegisterType<AccountRepository>().As<IAccountRepository>().InstancePerDependency();
    builder.RegisterType<CarrierReturnReportRepository>().As<ICarrierReturnReport>().InstancePerDependency();
    builder.RegisterType<EmployeeRepository>().As<IEmployeeRepository>().InstancePerDependency();
    builder.RegisterType<PerformanceReportRepository>().As<IPerformanceReportRepository>().InstancePerDependency();
    builder.RegisterType<ExpenseRepository>().As<IExpenseRepository>().InstancePerDependency();
    builder.RegisterType<PaymentProcessRepository>().As<IPaymentProcessRepository>().InstancePerDependency();
    builder.RegisterType<StripeRepository>().As<IStripeRepository>().InstancePerDependency();
    builder.RegisterType<SaleChannelConfigRepository>().As<ISaleChannelConfigRepository>().InstancePerDependency();
    builder.RegisterType<ShopifyRepository>().As<IShopifyRepository>().InstancePerDependency();
    builder.RegisterType<ShopifyUrlsRepository>().As<IShopifyUrlsRepository>().InstancePerDependency();
    builder.RegisterType<ExpenseCategoryRepository>().As<IExpenseCategoryRepository>().InstancePerDependency();
    builder.RegisterType<SMSProcessRepository>().As<ISMSProcessRepository>().InstancePerDependency();
    builder.RegisterType<MobileServicesRepository>().As<IMobileServicesRepository>().InstancePerDependency();
    builder.RegisterType<AppConfigRepository>().As<IAppConfigRepository>().InstancePerDependency();
    builder.RegisterType<TaxRepository>().As<ITaxRepository>().InstancePerDependency();
    builder.RegisterType<KeyGeneratorManager>().As<IKeyGeneratorManager>().InstancePerDependency();
    builder.RegisterType<NotificationRepository>().As<INotificationRepository>().InstancePerDependency();
    builder.RegisterType<ReturnRepository>().As<IReturnRepository>().InstancePerDependency();
    builder.RegisterType<OrderBoxRepository>().As<IOrderBoxRepository>().InstancePerDependency();
    builder.RegisterType<WebHookEventRepository>().As<IWebHookEventRepository>().InstancePerDependency();
    builder.RegisterType<AwsCongnitoRepository>().As<IAwsCongnitoRepository>().InstancePerDependency();
    builder.RegisterType<WalletRepository>().As<IWalletRepository>().InstancePerDependency();
    builder.RegisterType<MasterDbRepository>().As<IMasterDbRepository>().InstancePerDependency();
    builder.RegisterType<WhatsAppThirdPartyRepository>().As<IWhatsAppThirdPartyRepository>().InstancePerDependency();
    builder.RegisterType<MetaFieldRepository>().As<IMetaFieldRepository>().InstancePerDependency();
    builder.RegisterType<ShipperInvoiceRepository>().As<IShipperInvoiceRepository>().InstancePerDependency();
    builder.RegisterType<interaktService>().As<IinteraktService>().InstancePerDependency();
    builder.RegisterType<ZokoService>().As<IZokoService>().InstancePerDependency();
    #region Sale Chanels Unified
    // Sale Channel Order Pre-Processor Strategy registrations.
    // Each implementation is keyed by EnumSaleChannelLookup int value so the handler
    // can resolve the correct strategy via IIndex<int, ISaleChannelOrderPreProcessorService>
    // without any if/else branching. To add a new sale channel, register one new line below.
    builder.RegisterType<Shipra.Backend.API.Infrastructure.Services.Shopify.ShopifyOrderPreProcessorService>()
      .As<IShopifyOrderPreProcessorService>()
      .As<ISaleChannelOrderPreProcessorService>()
      .Keyed<ISaleChannelOrderPreProcessorService>((int)EnumSaleChannelLookup.Shopify)
      .InstancePerDependency();

    builder.RegisterType<Shipra.Backend.API.Infrastructure.Services.WooCommerce.WooCommerceOrderPreProcessorService>()
      .As<IWooCommerceOrderPreProcessorService>()
      .As<ISaleChannelOrderPreProcessorService>()
      .Keyed<ISaleChannelOrderPreProcessorService>((int)EnumSaleChannelLookup.WooCommerce)
      .InstancePerDependency();

    builder.RegisterType<Shipra.Backend.API.Infrastructure.Services.Amazon.AmazonOrderPreProcessorService>()
      .As<IAmazonOrderPreProcessorService>()
      .As<ISaleChannelOrderPreProcessorService>()
      .Keyed<ISaleChannelOrderPreProcessorService>((int)EnumSaleChannelLookup.Amazon)
      .InstancePerDependency();

    builder.RegisterType<Shipra.Backend.API.Infrastructure.Services.Noon.NoonOrderPreProcessorService>()
      .As<ISaleChannelOrderPreProcessorService>()
      .Keyed<ISaleChannelOrderPreProcessorService>((int)EnumSaleChannelLookup.Noon)
      .InstancePerDependency();

    // Sale Channel Product Pre-Processors
    builder.RegisterType<Shipra.Backend.API.Infrastructure.Services.Shopify.ShopifyProductPreProcessorService>()
      .As<ISaleChannelProductPreProcessorService>()
      .Keyed<ISaleChannelProductPreProcessorService>((int)EnumSaleChannelLookup.Shopify)
      .InstancePerDependency();

    builder.RegisterType<Shipra.Backend.API.Infrastructure.Services.WooCommerce.WooCommerceProductPreProcessorService>()
      .As<ISaleChannelProductPreProcessorService>()
      .Keyed<ISaleChannelProductPreProcessorService>((int)EnumSaleChannelLookup.WooCommerce)
      .InstancePerDependency();

    builder.RegisterType<Shipra.Backend.API.Infrastructure.Services.Amazon.AmazonProductPreProcessorService>()
      .As<ISaleChannelProductPreProcessorService>()
      .Keyed<ISaleChannelProductPreProcessorService>((int)EnumSaleChannelLookup.Amazon)
      .InstancePerDependency();

    builder.RegisterType<Shipra.Backend.API.Infrastructure.Services.Noon.NoonProductPreProcessorService>()
      .As<ISaleChannelProductPreProcessorService>()
      .Keyed<ISaleChannelProductPreProcessorService>((int)EnumSaleChannelLookup.Noon)
      .InstancePerDependency();

    // Sale Channel Order Post-Processors
    builder.RegisterType<Shipra.Backend.API.Infrastructure.Services.Shopify.ShopifyOrderPostProcessorService>()
      .As<ISaleChannelOrderPostProcessorService>()
      .Keyed<ISaleChannelOrderPostProcessorService>((int)EnumSaleChannelLookup.Shopify)
      .InstancePerDependency();

    builder.RegisterType<Shipra.Backend.API.Infrastructure.Services.Noon.NoonOrderPostProcessorService>()
      .As<ISaleChannelOrderPostProcessorService>()
      .Keyed<ISaleChannelOrderPostProcessorService>((int)EnumSaleChannelLookup.Noon)
      .InstancePerDependency();

    // Sale Channel Product Post-Processors
    builder.RegisterType<Shipra.Backend.API.Infrastructure.Services.Shopify.ShopifyProductPostProcessorService>()
      .As<ISaleChannelProductPostProcessorService>()
      .Keyed<ISaleChannelProductPostProcessorService>((int)EnumSaleChannelLookup.Shopify)
      .InstancePerDependency();

    builder.RegisterType<Shipra.Backend.API.Infrastructure.Services.WooCommerce.WooCommerceProductPostProcessorService>()
      .As<ISaleChannelProductPostProcessorService>()
      .Keyed<ISaleChannelProductPostProcessorService>((int)EnumSaleChannelLookup.WooCommerce)
      .InstancePerDependency();

    builder.RegisterType<Shipra.Backend.API.Infrastructure.Services.Amazon.AmazonProductPostProcessorService>()
      .As<ISaleChannelProductPostProcessorService>()
      .Keyed<ISaleChannelProductPostProcessorService>((int)EnumSaleChannelLookup.Amazon)
      .InstancePerDependency();

    builder.RegisterType<Shipra.Backend.API.Infrastructure.Services.Noon.NoonProductPostProcessorService>()
      .As<ISaleChannelProductPostProcessorService>()
      .Keyed<ISaleChannelProductPostProcessorService>((int)EnumSaleChannelLookup.Noon)
      .InstancePerDependency();

    #endregion

    #region sale channel 
    builder.RegisterType<SaleChannelFactory>().As<ISaleChannelFactory>().InstancePerDependency();
    builder.RegisterType<ShopifyService>().AsSelf().InstancePerDependency();
    builder.RegisterType<JobRepository>().As<IJobRepository>().InstancePerDependency();
    builder.RegisterType<SharedCreateOrderShipraRepository>().As<ISharedCreateOrderShipraRepository>().InstancePerDependency();

    #endregion

    #region db context
    builder.RegisterType<DbContextService>().As<IDbContextService>().InstancePerDependency();
    builder.RegisterType<CurrentTenantRepository>().As<ICurrentTenantRepository>().InstancePerDependency();
    #endregion
    #region sms
    builder.RegisterType<SmsRepository>().As<ISmsRepository>().InstancePerDependency();
    builder.RegisterType<SmsProviderFactory>().As<ISmsProviderFactory>().InstancePerDependency();
    builder.RegisterType<SmsService>().As<ISmsService>().InstancePerDependency();
    builder.RegisterType<DocumentRepository>().As<IDocumentRepository>().InstancePerDependency();
    builder.RegisterType<SharedBarcodeRepository>().As<ISharedBarcodeRepository>().InstancePerDependency();
    #endregion
    #region permission
    builder.RegisterType<DynamicPermissionService>().As<IDynamicPermissionService>().InstancePerDependency();
    builder.RegisterType<PermissionRepository>().As<IPermissionRepository>().InstancePerDependency();
    builder.RegisterType<ClientRepositoryInitializer>().As<IClientRepositoryInitializer>().InstancePerDependency();
    builder.RegisterType<CatalogueRepository>().As<ICatalogueRepository>().InstancePerDependency();
    builder.RegisterType<ShopifyPluginRepository>().As<IShopifyPluginRepository>().InstancePerDependency();

    #endregion 
    #region shared depenedency
    builder.RegisterType<CarrierSharedRepository>().As<ICarrierSharedRepository>().InstancePerDependency();
    builder.RegisterType<SharedStripeRepository>().As<ISharedStripeRepository>().InstancePerDependency();
    builder.RegisterType<SharedUserManagementRepository>().As<ISharedUserManagement>().InstancePerDependency();
    builder.RegisterType<SharedTotalProcessingRepository>().As<ISharedTotalProcessingRepository>().InstancePerDependency();
    #endregion
    //services register

    builder.RegisterType<StoreRepository>().As<IStoreRepository>().InstancePerDependency();
    builder.RegisterType<ActivityLogRepository>().As<IActivityLogRepository>().InstancePerDependency();
  }
  private void RegisterProductionOnlyDependencies(ContainerBuilder builder)
  {
    // NOTE: Add any production only services here
    builder.RegisterType<SmtpEmailSender>().As<IEmailSender>()
      .InstancePerDependency();
    // NOTE: Add any development only services here
    RegisterDependencyForBothEnvironment(builder);
  }
}
