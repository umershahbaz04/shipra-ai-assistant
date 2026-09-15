using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.CountryAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class ZoneConfiguration : IEntityTypeConfiguration<Zone>
{
  public void Configure(EntityTypeBuilder<Zone> builder)
  {
    builder.HasKey(x => x.ZoneId);
    builder.ToTable("Zone");
    builder.Property(e => e.Code).HasMaxLength(200);
    builder.Property(e => e.Name).HasMaxLength(200);
    builder.Property(e => e.NameArabic).HasMaxLength(200);
    builder.Property(e => e.CityID).HasColumnName("CityID");
    builder.Property(e => e.Coords);
    builder.Property(e => e.Active).HasDefaultValue(true);
  }
}
