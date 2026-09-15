using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class OrderTaxConfiguration : IEntityTypeConfiguration<OrderTax>
{
  public void Configure(EntityTypeBuilder<OrderTax> builder)
  {
    builder.ToTable("OrderTax");
    #region conversion
    builder.Property(e => e.OrderTaxId).HasConversion(uuid => uuid!.Value, value => new OrderTaxId(value));
    builder.Property(e => e.OrderId).HasConversion(uuid => uuid!.Value, value => new OrderId(value));
    #endregion
    builder.Property(e => e.OrderTaxId).ValueGeneratedNever();
    builder.Property(e => e.TaxValue).HasColumnType("decimal(10, 2)");
  }
}
