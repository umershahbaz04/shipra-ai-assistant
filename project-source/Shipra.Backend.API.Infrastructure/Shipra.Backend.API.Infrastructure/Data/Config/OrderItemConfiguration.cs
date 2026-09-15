using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
  public void Configure(EntityTypeBuilder<OrderItem> builder)
  {
    builder.HasKey(e => e.OrderItemId);
    builder.ToTable("OrderItem");

    #region conversions
    builder.Property(e => e.OrderItemId).HasConversion(uuid => uuid!.Value, value => new OrderItemId(value));
    builder.Property(e => e.OrderId).HasConversion(uuid => uuid!.Value, value => new OrderId(value));
    builder.Property(e => e.ProductId).HasConversion(uuid => uuid!.Value, value => new ProductId(value));
    #endregion

    builder.Property(e => e.OrderItemId).ValueGeneratedNever();
    builder.Property(e => e.ProductVariantId);
    builder.Property(e => e.Description).HasMaxLength(300);
    builder.Property(e => e.Discount).HasColumnType("decimal(18, 2)");
    builder.Property(e => e.Price).HasColumnType("numeric(18, 2)");
    builder.Property(e => e.ItemBarcode).HasMaxLength(50);
    builder.Property(e => e.Remarks).HasMaxLength(300);
  }
}
