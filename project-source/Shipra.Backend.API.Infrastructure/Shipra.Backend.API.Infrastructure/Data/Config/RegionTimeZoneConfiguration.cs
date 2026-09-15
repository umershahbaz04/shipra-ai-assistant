using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.CommonAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class RegionTimeZoneConfiguration : IEntityTypeConfiguration<RegionTimeZone>
{
  public void Configure(EntityTypeBuilder<RegionTimeZone> builder)
  {
    builder.HasKey(e => e.RegionTimeZoneId).HasName("PK_TimeZone");

    builder.ToTable("RegionTimeZone");

    builder.Property(e => e.HoursDifference).HasMaxLength(50);
    builder.Property(e => e.TimeZoneName).HasMaxLength(100);
  }
}
