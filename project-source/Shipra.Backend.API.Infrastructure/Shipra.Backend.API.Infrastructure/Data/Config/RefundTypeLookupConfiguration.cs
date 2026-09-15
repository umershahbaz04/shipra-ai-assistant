using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ReturnAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class RefundTypeLookupConfiguration : IEntityTypeConfiguration<RefundTypeLookup>
{
  public void Configure(EntityTypeBuilder<RefundTypeLookup> builder)
  {
    builder.HasKey(e => e.RefundTypeId);

    builder.ToTable("RefundTypeLookup");

    builder.Property(e => e.RefundTypeName)
        .HasMaxLength(20)
        .IsUnicode(false);
  }
}
