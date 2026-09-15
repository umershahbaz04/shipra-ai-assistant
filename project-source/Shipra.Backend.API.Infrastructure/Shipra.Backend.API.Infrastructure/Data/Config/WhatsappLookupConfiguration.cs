using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.SMSProcessAggregate;
using Shipra.Backend.API.Core.WhatsappAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class WhatsappLookupConfiguration : IEntityTypeConfiguration<WhatsappLookup>
{
  public void Configure(EntityTypeBuilder<WhatsappLookup> builder)
  {
    builder.ToTable("WhatsappLookup");

    builder.Property(e => e.WhatsAppLookupId)
        .ValueGeneratedNever()
        .HasColumnName("WhatsAppLookupId");
    builder.Property(e => e.InputRequiredConfig).HasColumnType("text");
    builder.Property(e => e.ServiceName).HasMaxLength(50);
  }
}
