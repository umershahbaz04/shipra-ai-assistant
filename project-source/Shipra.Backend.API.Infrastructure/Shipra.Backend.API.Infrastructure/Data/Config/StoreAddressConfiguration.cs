using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.StoresAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class StoreAddressConfiguration : IEntityTypeConfiguration<StoreAddress>
{
  public void Configure(EntityTypeBuilder<StoreAddress> builder)
  {
    builder.ToTable("StoreAddress");

    builder.Property(e => e.BuildingName).HasMaxLength(50);
    builder.Property(e => e.FullAddress).HasMaxLength(200);
    builder.Property(e => e.HouseNo).HasMaxLength(20);
    builder.Property(e => e.Landmark).HasMaxLength(50);
    builder.Property(e => e.Latitude).HasColumnType("decimal(8, 6)");
    builder.Property(e => e.Longitude).HasColumnType("decimal(9, 6)");
    builder.Property(e => e.StreetAddress).HasMaxLength(150);
    builder.Property(e => e.StreetAddress2).HasMaxLength(500);
    builder.Property(e => e.Zip).HasMaxLength(10);
  }
}

