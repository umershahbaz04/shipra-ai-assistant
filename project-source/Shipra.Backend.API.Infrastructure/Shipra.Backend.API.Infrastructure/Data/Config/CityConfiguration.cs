using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.CountryAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class CityConfiguration : IEntityTypeConfiguration<City>
{
  public void Configure(EntityTypeBuilder<City> builder)
  {
    builder.HasKey(x => x.CityId);
    builder.ToTable("City");
    builder.Property(e => e.Code).HasMaxLength(20);
    builder.Property(e => e.Name).HasMaxLength(130);
    builder.Property(e => e.NameArabic).HasMaxLength(120);
    //builder.Property(e => e.CountryId).HasMaxLength(36);
    // builder.Property(e => e.ExtendId).HasMaxLength(36);
  }
}
