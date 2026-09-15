using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class ClientGenericSettingLookupConfiguration : IEntityTypeConfiguration<ClientGenericSettingLookup>
{
  public void Configure(EntityTypeBuilder<ClientGenericSettingLookup> builder)
  {
    builder.ToTable("ClientGenericSettingLookup"); 
  }
}
