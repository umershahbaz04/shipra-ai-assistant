using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.CommonAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class FullFillmentStatusLookupConfiguration : IEntityTypeConfiguration<FullFillmentStatusLookup>
{
  public void Configure(EntityTypeBuilder<FullFillmentStatusLookup> builder)
  {
    builder.HasKey(e => e.FullFillmentStatusId);
    builder.ToTable("FullFillmentStatusLookup");
    builder.Property(e => e.FullFillmentStatus).HasMaxLength(100);
  }
}
