using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class StationLookupConfiguration : IEntityTypeConfiguration<StationLookup>
{ 
  public void Configure(EntityTypeBuilder<StationLookup> builder)
  {
    builder.HasKey(e => e.StationId).HasName("PK_Station");

    builder.ToTable("StationLookup");

    builder.Property(e => e.Sname)
        .HasMaxLength(50)
        .HasColumnName("SName");
  }
}
