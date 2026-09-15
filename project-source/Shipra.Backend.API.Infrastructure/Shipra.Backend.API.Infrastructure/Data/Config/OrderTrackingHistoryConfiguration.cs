using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class OrderTrackingHistoryConfiguration : IEntityTypeConfiguration<OrderTrackingHistory>
{
  public void Configure(EntityTypeBuilder<OrderTrackingHistory> builder)
  {
    builder.HasKey(e => e.OrderTrackingHistoryId);
    builder.ToTable("OrderTrackingHistory");

    #region conversion
    builder.Property(e => e.OrderTrackingHistoryId).HasConversion(orderId => orderId!.Value, value => new OrderTrackingHistoryId(value!));
    builder.Property(e => e.OrderId).HasConversion(orderId => orderId!.Value, value => new OrderId(value!)); 
    builder.Property(e => e.CreatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    #endregion
    builder.Property(e => e.Location).HasMaxLength(250);
    builder.Property(e => e.Latitude).HasColumnType("decimal(8, 6)");
    builder.Property(e => e.Longitude).HasColumnType("decimal(9, 6)");
    builder.Property(e => e.OrderTrackingHistoryId).ValueGeneratedNever();
    builder.Property(e => e.CreatedOn).HasColumnType("datetime");
  }
}
