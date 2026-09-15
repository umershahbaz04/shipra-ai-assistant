using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.TaxAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class TaxTypeLookupConfiguration : IEntityTypeConfiguration<TaxTypeLookup>
{
  public void Configure(EntityTypeBuilder<TaxTypeLookup> builder)
  {
    builder.HasKey(e => e.TaxId);

    builder.ToTable("TaxTypeLookup");

    builder.Property(e => e.Name).HasMaxLength(50);
    builder.Property(e => e.Percentage).HasColumnType("decimal(10, 2)");
  }
}
