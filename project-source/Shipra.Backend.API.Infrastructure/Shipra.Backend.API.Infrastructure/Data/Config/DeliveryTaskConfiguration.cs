using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.DeliveryTaskAggregate;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.DeliveryNoteAggregate;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class DeliveryTaskConfiguration : IEntityTypeConfiguration<DeliveryTask>
{
  public void Configure(EntityTypeBuilder<DeliveryTask> entity)
  {
    entity.HasKey(e => e.DeliveryTaskId).HasName("PK_ShipmentDeliveryJob");


    entity.HasKey(e => e.DeliveryTaskId);
    entity.ToTable("DeliveryTask");
    #region conversion

    entity.Property(e => e.DeliveryTaskId).HasConversion(deliveryTaskId => deliveryTaskId!.Value, value => new DeliveryTaskId(value!));
    entity.Property(e => e.OrderId).HasConversion(orderId => orderId!.Value, value => new OrderId(value!));
    entity.Property(e => e.ClientId).HasConversion(id => id!.Value, value => new ClientId(value!));
    entity.Property(e => e.DriverId).HasConversion(driverId => driverId!.Value, value => new DriverId(value!));
    entity.Property(e => e.DriverReceivableId).HasConversion(driverReceivableId => driverReceivableId!.Value, value => new DriverReceivableId(value!));
    entity.Property(e => e.DeliveryNoteDetailId).HasConversion(id => id!.value, value => new DeliveryNoteDetailId(value!));
    entity.Property(e => e.UpdatedBy).HasConversion(id => id!.Value, value => new EmployeeId(value!));
    entity.Property(e => e.CreatedBy).HasConversion(id => id!.Value, value => new EmployeeId(value!));

    #endregion

    entity.Property(e => e.DeliveryTaskId).ValueGeneratedNever();
    entity.Property(e => e.CreatedOn).HasColumnType("datetime");
    entity.Property(e => e.DriverPaidDate).HasColumnType("datetime");
    entity.Property(e => e.JobCode).HasMaxLength(50);
    entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
  }
}
