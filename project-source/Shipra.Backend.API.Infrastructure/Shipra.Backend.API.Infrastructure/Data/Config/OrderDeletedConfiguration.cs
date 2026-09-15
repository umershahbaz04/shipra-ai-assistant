using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.AccountAggregate;
using Shipra.Backend.API.Core.CarrierReturnReportAggregate;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.SaleChannelOrderAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class OrderDeletedConfiguration : IEntityTypeConfiguration<OrderDeleted>
{
  public void Configure(EntityTypeBuilder<OrderDeleted> builder)
  {
    builder.HasKey(x => x.OrderId);
    builder.ToTable("OrderDeleted");
    #region conversion
    builder.Property(e => e.OrderDeletedId).HasConversion(orderId => orderId!.Value, value => new OrderDeletedId(value!));
    builder.Property(e => e.OrderId).HasConversion(orderId => orderId!.Value, value => new OrderId(value!));
    builder.Property(e => e.ClientId).HasConversion(orderId => orderId!.Value, value => new ClientId(value!));
    builder.Property(e => e.SaleChannelOrderId).HasConversion(saleChannelOrderId => saleChannelOrderId!.Value, value => new SaleChannelOrderId(value!));

    builder.Property(e => e.CarrierPaymentSettlementId).HasConversion(uuid => uuid!.Value, value => new CarrierPaymentSettlementId(value));
    builder.Property(e => e.CreatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    builder.Property(e => e.UpdatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    builder.Property(e => e.FulFiledById).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    builder.Property(e => e.CarrierRRId).HasConversion(uuid => uuid!.Value, value => new CarrierRRId(value));

    #endregion

    builder.Property(e => e.OrderId).ValueGeneratedNever();
    builder.Property(e => e.Amount).HasColumnType("numeric(18, 2)");
    builder.Property(e => e.CarrierLastUpdateDateTime).HasColumnType("datetime");
    builder.Property(e => e.FulFilledDate).HasColumnType("datetime");
    builder.Property(e => e.CarrierAssignDate).HasColumnType("datetime");
    builder.Property(e => e.CarrierTrackingNo).HasMaxLength(20);
    builder.Property(e => e.CarrierTrackingStatus).HasMaxLength(250);
    builder.Property(e => e.CreatedOn).HasColumnType("datetime");
    builder.Property(e => e.DeliveryCharges).HasColumnType("decimal(18, 2)");
    builder.Property(e => e.Description).HasMaxLength(500);
    builder.Property(e => e.Discount).HasColumnType("decimal(18, 2)");
    builder.Property(e => e.ItemValue).HasColumnType("decimal(18, 2)");
    builder.Property(e => e.OrderDate).HasColumnType("datetime");
    builder.Property(e => e.OrderNo).HasMaxLength(20);
    builder.Property(e => e.PaymentRef).HasMaxLength(50);
    builder.Property(e => e.Remarks).HasMaxLength(500);
    builder.Property(e => e.ReturnRef).HasMaxLength(50);
    builder.Property(e => e.UpdatedOn).HasColumnType("datetime");
    builder.Property(e => e.Vat)
        .HasColumnType("decimal(18, 2)")
        .HasColumnName("VAT");
    builder.Property(e => e.Weight).HasColumnType("decimal(18, 2)");


  }
}
