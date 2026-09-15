using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.SaleChannelConfigAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class SaleChannelLookupConfiguration : IEntityTypeConfiguration<SaleChannelLookup>
{
  public void Configure(EntityTypeBuilder<SaleChannelLookup> builder)
  {
    builder.HasKey(e => e.SaleChannelLookupId);
    builder.ToTable("SaleChannelLookup");
    builder.Property(e => e.InputRequiredConfig).HasColumnType("text");
    builder.Property(e => e.SaleChannelName).HasMaxLength(50);
  }
}
