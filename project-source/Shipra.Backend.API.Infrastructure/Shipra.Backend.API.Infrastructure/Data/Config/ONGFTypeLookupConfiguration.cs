using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.CommonAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class ONGFTypeLookupConfiguration : IEntityTypeConfiguration<ONGFTypeLookup>
{
  public void Configure(EntityTypeBuilder<ONGFTypeLookup> builder)
  {
    builder.HasKey(e => e.ONGFTypeId);
    builder.ToTable("ONGFTypeLookup");
    builder.Property(e => e.Type).HasMaxLength(50);
    builder.Property(e => e.Description).HasMaxLength(150);
  }
}

