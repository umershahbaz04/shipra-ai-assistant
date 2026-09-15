using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
internal class OrderTypeLookupConfiguration : IEntityTypeConfiguration<OrderTypeLookup>
{
  public void Configure(EntityTypeBuilder<OrderTypeLookup> builder)
  {
    builder.HasKey(e => e.OrderTypeId).HasName("PK_OrderType");

    builder.ToTable("OrderTypeLookup");

    builder.Property(e => e.OrderTypeName)
        .HasMaxLength(50)
        .HasColumnName("OrderTypeName");
  }
}
