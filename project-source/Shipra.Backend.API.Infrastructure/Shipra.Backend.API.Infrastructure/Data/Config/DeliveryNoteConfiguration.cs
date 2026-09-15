using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.DeliveryNoteAggregate;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class DeliveryNoteConfiguration : IEntityTypeConfiguration<DeliveryNote>
{
  public void Configure(EntityTypeBuilder<DeliveryNote> entity)
  {

    entity.HasKey(e => e.DeliveryNoteId);
    entity.ToTable("DeliveryNote");

    #region conversion

    entity.Property(e => e.DeliveryNoteId).HasConversion(deliveryNoteId => deliveryNoteId!.Value, value => new DeliveryNoteId(value!));
    entity.Property(e => e.DriverId).HasConversion(driverId => driverId!.Value, value => new DriverId(value!));
    entity.Property(e => e.ClientId).HasConversion(clientId => clientId!.Value, value => new ClientId(value!));
    entity.Property(e => e.UpdatedBy).HasConversion(id => id!.Value, value => new EmployeeId(value!));
    entity.Property(e => e.CreatedBy).HasConversion(id => id!.Value, value => new EmployeeId(value!));

    #endregion

    entity.Property(e => e.DeliveryNoteId).ValueGeneratedNever();
    entity.Property(e => e.IsCompleted).HasDefaultValueSql("((1))");
    entity.Property(e => e.Active).HasDefaultValueSql("((1))");
    entity.Property(e => e.CreatedOn).HasColumnType("datetime");
    entity.Property(e => e.NoteNo).HasMaxLength(20);
    entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
  }
}
