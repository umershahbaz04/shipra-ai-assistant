using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.CountryAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class AreaConfiguration : IEntityTypeConfiguration<Area>
{
  public void Configure(EntityTypeBuilder<Area> builder)
  {
    builder.HasKey(x => x.AreaId);
    builder.ToTable("Area");
    builder.Property(e => e.Code).HasMaxLength(20);
    builder.Property(e => e.Latitude).HasColumnType("decimal(8, 6)");
    builder.Property(e => e.Longitude).HasColumnType("decimal(9, 6)");
    builder.Property(e => e.Name).HasMaxLength(250);
    builder.Property(e => e.NameArabic).HasMaxLength(250);
  }
}
