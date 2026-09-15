using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.WhatsappAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class WhatsappActivateConfiguration : IEntityTypeConfiguration<WhatsappActivate>
{
  public void Configure(EntityTypeBuilder<WhatsappActivate> builder)
  {
    builder.ToTable("WhatsappActivate");
    builder.HasKey(e => e.WhatsAppActivateId);
    #region conversion 
    builder.Property(e => e.ClientId).HasConversion(guid => guid!.Value, value => new ClientId(value!));
    builder.Property(e => e.CreatedBy).HasConversion(guid => guid!.Value, value => new EmployeeId(value!));
    builder.Property(e => e.UpdatedBy).HasConversion(guid => guid!.Value, value => new EmployeeId(value!));
    #endregion
    builder.ToTable("WhatsappActivate");

    builder.Property(e => e.WhatsAppActivateId)
        .HasColumnName("WhatsAppActivateId");
    builder.Property(e => e.Active).HasDefaultValueSql("((1))");
    builder.Property(e => e.Config).HasColumnType("text");
    builder.Property(e => e.CreatedOn).HasColumnType("datetime");
    builder.Property(e => e.WhatsAppLookupId).HasColumnName("WhatsAppLookupId");
    builder.Property(e => e.UpdateOn).HasColumnType("datetime");
  }
}
