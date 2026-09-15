using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class OrderAddressConfiguration : IEntityTypeConfiguration<OrderAddress>
{
  public void Configure(EntityTypeBuilder<OrderAddress> builder)
  {
    builder.ToTable("OrderAddress");

    builder.Property(e => e.CustomerFullAddress).HasMaxLength(500);
    builder.Property(e => e.CustomerName).HasMaxLength(255);
    builder.Property(e => e.Email).HasMaxLength(100);
    builder.Property(e => e.Latitude).HasColumnType("decimal(8, 6)");
    builder.Property(e => e.Longitude).HasColumnType("decimal(9, 6)");
    builder.Property(e => e.Mobile1).HasMaxLength(20);
    builder.Property(e => e.Mobile2).HasMaxLength(20);
    builder.Property(e => e.StreetAddress).HasMaxLength(500);

  }
}
