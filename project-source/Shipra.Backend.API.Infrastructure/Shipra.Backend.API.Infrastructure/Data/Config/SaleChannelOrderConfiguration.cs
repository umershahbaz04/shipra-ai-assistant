using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.SaleChannelOrderAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class SaleChannelOrderConfiguration : IEntityTypeConfiguration<SaleChannelOrder>
{
  public void Configure(EntityTypeBuilder<SaleChannelOrder> builder)
  {
    builder.ToTable("SaleChannelOrder");

    #region conversion 
    builder.Property(e => e.SaleChannelOrderId).HasConversion(guid => guid!.Value, value => new SaleChannelOrderId(value!));
    builder.Property(e => e.ClientId).HasConversion(guid => guid!.Value, value => new ClientId(value!));
    builder.Property(e => e.CreatedBy).HasConversion(guid => guid!.Value, value => new EmployeeId(value!));
    builder.Property(e => e.ShipraOrderId).HasConversion(guid => guid!.Value, value => new OrderId(value!));
    #endregion


    builder.Property(e => e.SaleChannelOrderId)
        .ValueGeneratedOnAdd()
        .HasColumnName("SaleChannelOrderId");
    builder.Property(e => e.SaleChannelLookupId).HasColumnName("SaleChannelLookupId");
    builder.Property(e => e.OrderId).HasMaxLength(50);
    builder.Property(e => e.OrderJson).HasColumnType("text");
    builder.Property(e => e.OrderNo).HasMaxLength(50);

  }
}
