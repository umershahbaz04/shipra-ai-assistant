using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.MetaFieldAggregate;
using Shipra.Backend.API.Core.WhatsappAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class EntityMetaFieldLookupConfiguration : IEntityTypeConfiguration<EntityMetaFieldLookup>
{
  public void Configure(EntityTypeBuilder<EntityMetaFieldLookup> builder)
  {
    builder.ToTable("EntityMetaFieldLookup");

    builder.HasKey(e => e.EntityMetaFieldId);

    builder.Property(e => e.EntityMetaFieldId)
        .ValueGeneratedNever()
        .HasColumnName("EntityMetaFieldId");
    builder.Property(e => e.SettingConfig).HasColumnType("text");
    builder.Property(e => e.TypeName).HasMaxLength(50);
  }
}
