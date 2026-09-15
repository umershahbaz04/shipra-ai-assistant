using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.OrderBoxAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class BoxTypeLookupConfiguration : IEntityTypeConfiguration<BoxTypeLookup>
{
  public void Configure(EntityTypeBuilder<BoxTypeLookup> builder)
  {
    builder.ToTable("BoxTypeLookup");

    builder.Property(e => e.BoxName).HasMaxLength(50);
    builder.Property(e => e.Height).HasColumnType("decimal(18, 2)");
    builder.Property(e => e.Length).HasColumnType("decimal(18, 2)");
    builder.Property(e => e.Volume).HasColumnType("decimal(18, 2)");
    builder.Property(e => e.Width).HasColumnType("decimal(18, 2)");
  }
}
