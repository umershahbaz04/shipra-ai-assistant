using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.MetaFieldAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class MetaFieldConfiguration : IEntityTypeConfiguration<MetaField>
{
  public void Configure(EntityTypeBuilder<MetaField> builder)
  {
    builder.ToTable("MetaField");
    #region conversion 
    builder.Property(e => e.ClientId).HasConversion(guid => guid!.Value, value => new ClientId(value!));
    #endregion
    builder.HasKey(e => e.MetaFieldId);
    builder.Property(e => e.SettingConfig).HasColumnType("text");
    builder.Property(e => e.EntityId);
  }
}
