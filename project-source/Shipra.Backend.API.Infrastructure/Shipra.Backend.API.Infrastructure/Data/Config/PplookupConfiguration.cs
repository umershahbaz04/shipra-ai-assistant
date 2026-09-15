using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.PaymentProcessAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class PplookupConfiguration : IEntityTypeConfiguration<Pplookup>
{
  public void Configure(EntityTypeBuilder<Pplookup> builder)
  {
    builder.HasNoKey().ToTable("PPLookup");

    builder.Property(e => e.PplookupId)
        .ValueGeneratedOnAdd()
        .HasComment("PaymentProcessId")
        .HasColumnName("PPLookupId");
    builder.Property(e => e.Ppname)
        .HasMaxLength(50)
        .HasColumnName("PPName");
    builder.Property(e => e.InputRequiredConfig).HasColumnType("text"); 
  }
}
