using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.CountryAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
  public void Configure(EntityTypeBuilder<Country> builder)
  {

    builder.HasKey(e => e.CountryId);
    builder.ToTable("Country");
    builder.Property(e => e.Code).HasMaxLength(20);
    builder.Property(e => e.Name).HasMaxLength(30);
    builder.Property(e => e.NameArabic).HasMaxLength(50);
  }
}
