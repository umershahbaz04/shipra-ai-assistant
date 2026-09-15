using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ReturnAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class ReturnStatusLookupConfiguration : IEntityTypeConfiguration<ReturnStatusLookup>
{
  public void Configure(EntityTypeBuilder<ReturnStatusLookup> builder)
  {
    builder.ToTable("ReturnStatusLookup");

    builder.Property(e => e.ReturnStatus).HasMaxLength(50);
  }

}
