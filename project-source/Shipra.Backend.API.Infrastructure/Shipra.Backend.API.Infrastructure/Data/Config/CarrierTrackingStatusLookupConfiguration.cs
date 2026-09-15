using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.CommonAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class CarrierTrackingStatusLookupConfiguration : IEntityTypeConfiguration<CarrierTrackingStatusLookup>
{
  public void Configure(EntityTypeBuilder<CarrierTrackingStatusLookup> builder)
  {
    builder.ToTable("CarrierTrackingStatusLookup");
    builder.HasKey(e => e.CarrierTrackingStatusId).HasName("PK_ShipmentTrackingStatusOnInvoice_Available");
    builder.Property(e => e.TrackingStatus)
        .HasMaxLength(50)
        .IsUnicode(false);
    builder.Property(e => e.TrackingStatusAr).HasMaxLength(50);
  }
}
