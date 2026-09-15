using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.DeliveryNoteAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class DeliveryNoteDetailConfiguration : IEntityTypeConfiguration<DeliveryNoteDetail>
{
  public void Configure(EntityTypeBuilder<DeliveryNoteDetail> entity)
  {
    entity.HasKey(e => e.DeliveryNoteDetailId);
    entity.ToTable("DeliveryNoteDetail");
    #region conversion

    entity.Property(e => e.DeliveryNoteDetailId).HasConversion(deliveryNoteDetailId => deliveryNoteDetailId!.value, value => new DeliveryNoteDetailId(value!));
    entity.Property(e => e.DeliveryNoteId).HasConversion(deliveryNoteId => deliveryNoteId!.Value, value => new DeliveryNoteId(value!));
    entity.Property(e => e.OrderId).HasConversion(orderId => orderId!.Value, value => new OrderId(value!));
    entity.Property(e => e.UpdatedBy).HasConversion(id => id!.Value, value => new EmployeeId(value!));
    entity.Property(e => e.CreatedBy).HasConversion(id => id!.Value, value => new EmployeeId(value!));

    #endregion
    entity.Property(e => e.DeliveryNoteDetailId).ValueGeneratedNever();
    entity.Property(e => e.Active).HasDefaultValueSql("((1))");
    entity.Property(e => e.CreatedOn).HasColumnType("datetime");
    entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
  }
}
