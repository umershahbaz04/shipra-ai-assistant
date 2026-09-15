using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.ClientAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class ActiveCarrierPickupLocationConfiguration : IEntityTypeConfiguration<ActiveCarrierPickupLocation>
{
  public void Configure(EntityTypeBuilder<ActiveCarrierPickupLocation> builder)
  {
    builder.ToTable("ActiveCarrierPickupLocation"); 
    #region convrsion
    builder.Property(e => e.ClientId).HasConversion(uuid => uuid!.Value, value => new ClientId(value));
    #endregion

    builder.Property(e => e.AreaName).HasMaxLength(150);
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
