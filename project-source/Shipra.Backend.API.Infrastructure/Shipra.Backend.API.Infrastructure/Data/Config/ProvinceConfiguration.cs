using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.CountryAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class ProvinceConfiguration : IEntityTypeConfiguration<Province>
{
  public void Configure(EntityTypeBuilder<Province> builder)
  {
    builder.HasKey(e => e.ProvinceId).HasName("PK_Provice");

    builder.ToTable("Province");

    builder.Property(e => e.Name).HasMaxLength(50);
  }
}
