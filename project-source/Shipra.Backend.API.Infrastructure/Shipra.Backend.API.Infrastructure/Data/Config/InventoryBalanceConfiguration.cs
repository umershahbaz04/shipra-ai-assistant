using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class InventoryBalanceConfiguration : IEntityTypeConfiguration<InventoryBalance>
{
  public void Configure(EntityTypeBuilder<InventoryBalance> builder)
  {
      builder.HasKey(e => e.InventoryBalanceId);
      builder.ToTable("InventoryBalance");

      builder.Property(e => e.ProductVariantId).IsRequired();
      builder.Property(e => e.ProductStationId).IsRequired();
      builder.Property(e => e.QuantityOnHand).IsRequired().HasDefaultValue(0);
      builder.Property(e => e.QuantityCommitted).IsRequired().HasDefaultValue(0);
      builder.Property(e => e.QuantityIncoming).IsRequired().HasDefaultValue(0);
      builder.Property(e => e.QuantityAvailable).IsRequired().HasDefaultValue(0);
      builder.Property(e => e.QuantityDamaged).IsRequired().HasDefaultValue(0);
      builder.Property(e => e.QuantityOnOrder).IsRequired().HasDefaultValue(0);
      builder.Property(e => e.QuantityReserved).IsRequired().HasDefaultValue(0);

      builder.Property(e => e.LastExternalUpdateOn).HasColumnType("datetime");
      builder.Property(e => e.CreatedOn).HasColumnType("datetime").IsRequired().HasDefaultValueSql("GETDATE()");
      builder.Property(e => e.UpdatedOn).HasColumnType("datetime");
  }
}
