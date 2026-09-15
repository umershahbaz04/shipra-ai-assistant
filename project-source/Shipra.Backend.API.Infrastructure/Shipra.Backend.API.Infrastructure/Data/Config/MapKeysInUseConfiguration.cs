using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.MapKeyAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class MapKeysInUseConfiguration : IEntityTypeConfiguration<MapKeysInUse>
{
  public void Configure(EntityTypeBuilder<MapKeysInUse> builder)
  {
    builder.ToTable("MapKeysInUse");

    builder.Property(e => e.EndDate).HasColumnType("datetime");
    builder.Property(e => e.StartDate).HasColumnType("datetime");
  }
}

public class MapKeysHistoryConfiguration : IEntityTypeConfiguration<MapKeysHistory>
{
  public void Configure(EntityTypeBuilder<MapKeysHistory> builder)
  {
    builder.ToTable("MapKeysHistory");

    builder.Property(e => e.EndDate).HasColumnType("datetime");
    builder.Property(e => e.StartDate).HasColumnType("datetime");
  }
}

public class MapKeyConfiguration : IEntityTypeConfiguration<MapKey>
{
  public void Configure(EntityTypeBuilder<MapKey> builder)
  {
    builder.ToTable("MapKey");
  }
}
