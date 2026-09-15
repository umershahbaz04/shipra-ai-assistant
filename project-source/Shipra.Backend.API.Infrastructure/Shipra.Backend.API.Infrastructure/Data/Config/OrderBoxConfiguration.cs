using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.OrderBoxAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class OrderBoxConfiguration : IEntityTypeConfiguration<OrderBox>
{
  public void Configure(EntityTypeBuilder<OrderBox> builder)
  {
    builder.ToTable("OrderBox");

    #region conversion
    builder.Property(e => e.OrderId).HasConversion(orderId => orderId!.Value, value => new OrderId(value!));
    builder.Property(e => e.OrderBoxId).HasConversion(orderId => orderId!.Value, value => new OrderBoxId(value!));
    #endregion

    builder.Property(e => e.OrderBoxId).ValueGeneratedNever();

  }
}
