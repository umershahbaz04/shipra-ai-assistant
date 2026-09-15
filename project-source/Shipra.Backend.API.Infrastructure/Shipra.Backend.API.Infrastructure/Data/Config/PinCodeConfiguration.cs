using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.CountryAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class PinCodeConfiguration : IEntityTypeConfiguration<PinCode>
{
  public void Configure(EntityTypeBuilder<PinCode> builder)
  {
    builder.ToTable("PinCode");

    builder.Property(e => e.PinCodeValue).HasMaxLength(50);
  }
}
