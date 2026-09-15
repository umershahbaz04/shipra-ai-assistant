using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.ShipperInvoiceAggregate;

namespace Shipra.Backend.API.Infrastructure.Data;
public partial class ShipperInvoiceDbContext : DbContext
{
  public ShipperInvoiceDbContext()
  {
  }

  public ShipperInvoiceDbContext(DbContextOptions<ShipperInvoiceDbContext> options)
      : base(options)
  {
  }

  public virtual DbSet<ContractShipperRate> ContractShipperRates { get; set; }
  public virtual DbSet<ShipperInvoice> ShipperInvoices { get; set; }
  public virtual DbSet<ShipperInvoiceDetail> ShipperInvoiceDetails { get; set; }
  public virtual DbSet<ServiceRateGroup> ServiceRateGroups { get; set; }
  public virtual DbSet<ServiceRateGroupSlab> ServiceRateGroupSlabs { get; set; }
  public virtual DbSet<ShipperRate> ShipperRates { get; set; }
  public virtual DbSet<ShipperRateSlab> ShipperRateSlabs { get; set; }
  public virtual DbSet<ShipperInvoiceAdjustment> ShipperInvoiceAdjustments { get; set; }
  public virtual DbSet<ShipperOrder> ShipperOrders { get; set; }
  public virtual DbSet<InvoiceStatus> InvoiceStatuses { get; set; }
  public virtual DbSet<TransactionType> TransactionTypes { get; set; }


  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<ContractShipperRate>(entity =>
    {
      entity.HasKey(e => e.ContractShipperRatesId);

      entity.Property(e => e.ContractShipperRatesId)
            .ValueGeneratedOnAdd();

      entity.Property(e => e.CreatedDate)
            .HasDefaultValueSql("getdate()")
            .HasColumnType("datetime");
    });

    modelBuilder.Entity<ShipperInvoice>(entity =>
    {
      entity.HasKey(e => e.ShipperInvoiceId);
      entity.ToTable("ShipperInvoice");
    });

    modelBuilder.Entity<ShipperInvoiceDetail>(entity =>
    {
      entity.HasKey(e => e.ShipperInvoiceDetailId);
      entity.ToTable("ShipperInvoiceDetail");

      entity.Property(e => e.ShipperInvoiceDetailId)
            .ValueGeneratedOnAdd();
    });

    modelBuilder.Entity<ServiceRateGroup>(entity =>
    {
      entity.HasKey(e => e.ServiceRateGroupId);
      entity.ToTable("ServiceRateGroup");
    });

    modelBuilder.Entity<ServiceRateGroupSlab>(entity =>
    {
      entity.HasKey(e => e.ServiceRateGroupSlabId);
      entity.ToTable("ServiceRateGroupSlab");
    });

    modelBuilder.Entity<ShipperRate>(entity =>
    {
      entity.HasKey(e => e.ShipperRateId);
      entity.ToTable("ShipperRate");
    });

    modelBuilder.Entity<ShipperRateSlab>(entity =>
    {
      entity.HasKey(e => e.ShipperRateSlabId);
      entity.ToTable("ShipperRateSlab");
    });

    modelBuilder.Entity<ShipperInvoiceAdjustment>(entity =>
    {
      entity.HasKey(e => e.ShipperInvoiceAdjustmentId);
      entity.ToTable("ShipperInvoiceAdjustment");
    });
    modelBuilder.Entity<ShipperOrder>(entity =>
    {
      entity.HasKey(e => e.ShipperOrderId);
      entity.ToTable("ShipperOrder"); 
    }); 
    modelBuilder.Entity<InvoiceStatus>(entity =>
    {
      entity.HasKey(e => e.InvoiceStatusId);
      entity.ToTable("InvoiceStatus"); 
    });

    modelBuilder.Entity<TransactionType>(entity =>
    {
      entity.HasKey(e => e.TransactionTypeId);
      entity.ToTable("TransactionType");
    });


    OnModelCreatingPartial(modelBuilder);
  }


  partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
