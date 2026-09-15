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
public class ClientMetaFieldConfiguration : IEntityTypeConfiguration<ClientMetaField>
{
  public void Configure(EntityTypeBuilder<ClientMetaField> builder)
  {
    builder.ToTable("ClientMetaField");
    #region conversion 
    builder.Property(e => e.ClientId).HasConversion(guid => guid!.Value, value => new ClientId(value!));
    #endregion
    builder.HasKey(e => e.ClientMetaFieldId);
    builder.Property(e => e.SettingConfig).HasColumnType("text");
    builder.Property(e => e.ClientMetaFieldId).HasMaxLength(50);
  }
}
