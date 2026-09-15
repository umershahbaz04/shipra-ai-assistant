using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class ClientConfigSettingConfiguration : IEntityTypeConfiguration<ClientConfigSetting>
{
  public void Configure(EntityTypeBuilder<ClientConfigSetting> builder)
  { 
    builder.ToTable("ClientConfigSetting");
    builder.HasKey(e => e.ClientConfigSettingId);
    #region convrsion
    builder.Property(e => e.ClientId).HasConversion(uuid => uuid!.Value, value => new ClientId(value)); 
    #endregion
    
  }
}
