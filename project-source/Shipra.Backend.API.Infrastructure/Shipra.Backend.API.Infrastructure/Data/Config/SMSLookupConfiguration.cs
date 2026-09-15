using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.SMSProcessAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class SMSLookupConfiguration : IEntityTypeConfiguration<SMSLookup>
{

  public void Configure(EntityTypeBuilder<SMSLookup> builder)
  {
    builder.ToTable("SMSLookup");

    builder.Property(e => e.SMSLookupId)
        .ValueGeneratedNever()
        .HasColumnName("SMSLookupId");
    builder.Property(e => e.InputRequiredConfig).HasColumnType("text");
    builder.Property(e => e.ServiceName).HasMaxLength(50);
  }
  
}
