using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.CatalougeAggregate;
using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.ContractAggregate;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.MapKeyAggregate;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.SettingOperationDashboardAggregate;

namespace Shipra.Backend.API.Infrastructure.Data;
public partial class ShipraMasterDbContext : DbContext
{
  public ShipraMasterDbContext()
  {
  }

  public ShipraMasterDbContext(DbContextOptions<ShipraMasterDbContext> options)
      : base(options)
  {
  }

  public virtual DbSet<Area> Areas { get; set; } 
  public virtual DbSet<PinCode> PinCodes { get; set; } 
  public virtual DbSet<City> Cities { get; set; } 
  public virtual DbSet<Country> Countries { get; set; } 
  //public virtual DbSet<Region> Regions { get; set; } 
  public virtual DbSet<RegionTimeZone> RegionTimeZones { get; set; }
  public virtual DbSet<Mcconfig> Mcconfigs { get; set; }
  public virtual DbSet<Province> Provinces { get; set; }
  public virtual DbSet<Carrier> Carriers { get; set; }
  public virtual DbSet<CivilEntityType> CivilEntityTypes { get; set; }
  public DbSet<CarrierSla> CarrierSlas => Set<CarrierSla>(); 
  public virtual DbSet<CarrierFeature> CarrierFeatures { get; set; }
  public virtual DbSet<CarrierLocation> CarrierLocations { get; set; } 
  public virtual DbSet<CarrierDeliveryService> CarrierDeliveryServices { get; set; } 
  public virtual DbSet<DeliveryService> DeliveryServices { get; set; } 
  public virtual DbSet<DefaultShipmentDashboard> DefaultShipmentDashboards { get; set; }
  public virtual DbSet<CarrierTrackingStatusLookup> CarrierTrackingStatusLookups { get; set; } 
  public virtual DbSet<StationLookup> StationLookups { get; set; }
  public virtual DbSet<WebhookUrlHash> WebhookUrlHashes { get; set; }
  public virtual DbSet<MapKey> MapKeys { get; set; }
  public virtual DbSet<MapKeysHistory> MapKeysHistories { get; set; }
  public virtual DbSet<MapKeysInUse> MapKeysInUses { get; set; }
  public DbSet<State> States => Set<State>();
  //public DbSet<CivilEntityCarrierMapped> CivilEntityCarrierMappeds => Set<CivilEntityCarrierMapped>();
  public DbSet<CivilEntityExtended> CivilEntityExtendeds => Set<CivilEntityExtended>();
  public DbSet<ShipraContractCarrier> ShipraContractCarriers => Set<ShipraContractCarrier>();
  public DbSet<Catalogue> Catalogues => Set<Catalogue>();
  public DbSet<ShopifySession> ShopifySessions => Set<ShopifySession>();

  public DbSet<CatalogueDatabase> CatalogueDatabases => Set<CatalogueDatabase>();
  public DbSet<UOMLookup> UOMLookups => Set<UOMLookup>(); 
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder.EnableSensitiveDataLogging();
  }
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  { 
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }

  partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
