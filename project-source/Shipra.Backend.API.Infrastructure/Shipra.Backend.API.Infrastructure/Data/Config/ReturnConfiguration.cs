using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.CatalougeAggregate;
using System.Reflection.Emit;
using Shipra.Backend.API.Core.ReturnAggregate;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class ReturnConfiguration : IEntityTypeConfiguration<Return>
{
  public void Configure(EntityTypeBuilder<Return> builder)
  {
    #region convrsion
    builder.Property(e => e.ReturnId).HasConversion(uuid => uuid!.Value, value => new ReturnId(value));
    builder.Property(e => e.ClientId).HasConversion(uuid => uuid!.Value, value => new ClientId(value));
    builder.Property(e => e.OrderId).HasConversion(uuid => uuid!.Value, value => new OrderId(value));
    builder.Property(e => e.RtoorderId).HasConversion(uuid => uuid!.Value, value => new OrderId(value));
    builder.Property(e => e.ExchangeOrderId).HasConversion(uuid => uuid!.Value, value => new OrderId(value));
    #endregion
    builder.ToTable("Return");
    builder.Property(e => e.ReturnId).ValueGeneratedNever();
    builder.Property(e => e.RefundAmount).HasColumnType("decimal(18, 2)");
    builder.Property(e => e.ReturnCharges)
                .HasDefaultValueSql("((0))")
                .HasColumnType("decimal(18, 2)");
    builder.Property(e => e.ReturnComment).HasMaxLength(150);
    builder.Property(e => e.RtoorderId).HasColumnName("RTOOrderId");
  }
}


