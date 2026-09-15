using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.AppConfigAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class AppConfigConfiguration : IEntityTypeConfiguration<AppConfig>
{
  public void Configure(EntityTypeBuilder<AppConfig> builder)
  {
    builder.HasKey(x => x.AppConfigId);
    builder.ToTable("AppConfig");
    builder.Property(e => e.AppConfigKey).HasMaxLength(50);
  }
}
