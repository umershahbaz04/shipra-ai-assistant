using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class ClientGenericSettingConfiguration : IEntityTypeConfiguration<ClientGenericSetting>
{
  public void Configure(EntityTypeBuilder<ClientGenericSetting> builder)
  {
    builder.ToTable("ClientGenericSetting");
    #region conversion 
    builder.Property(e => e.ClientId).HasConversion(clientId => clientId!.Value, value => new ClientId(value!));  
    #endregion

  }
}
