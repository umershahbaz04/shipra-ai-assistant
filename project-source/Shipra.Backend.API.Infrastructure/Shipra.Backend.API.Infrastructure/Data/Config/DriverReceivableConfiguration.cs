using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.DeliveryNoteAggregate;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class DriverReceivableConfiguration : IEntityTypeConfiguration<DriverReceivable>
{
  public void Configure(EntityTypeBuilder<DriverReceivable> builder)
  {
    builder.HasKey(e => e.DriverReceivableId);
    builder.ToTable("DriverReceivable");

    #region conversion
    builder.Property(e => e.DriverReceivableId).HasConversion(uuid => uuid!.Value, value => new DriverReceivableId(value));
    builder.Property(e => e.DriverId).HasConversion(uuid => uuid!.Value, value => new DriverId(value));
    builder.Property(e => e.DeliveryNoteId).HasConversion(uuid => uuid!.Value, value => new DeliveryNoteId(value));
    builder.Property(e => e.CreatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    builder.Property(e => e.UpdatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    #endregion

    builder.Property(e => e.DriverReceivableId).ValueGeneratedNever();
    builder.Property(e => e.Cash).HasColumnType("decimal(18, 2)");
    builder.Property(e => e.CreatedOn).HasColumnType("datetime");
    builder.Property(e => e.Expense).HasColumnType("decimal(18, 2)");
    builder.Property(e => e.DriverReceivableNo).HasMaxLength(50);
    builder.Property(e => e.ReceiveDate).HasColumnType("datetime");
    builder.Property(e => e.Total).HasColumnType("decimal(18, 2)");
    builder.Property(e => e.UpdatedOn).HasColumnType("datetime");
  }
}
