using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.DeliveryNoteAggregate;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
  public void Configure(EntityTypeBuilder<Expense> builder)
  {
    builder.HasKey(e => e.ExpenseId);

    builder.ToTable("Expense");

    #region conversions
    builder.Property(e => e.ExpenseId).HasConversion(uuid => uuid!.Value, value => new ExpenseId(value));
    builder.Property(e => e.DriverId).HasConversion(uuid => uuid!.Value, value => new DriverId(value));
    builder.Property(e => e.ClientId).HasConversion(uuid => uuid!.Value, value => new ClientId(value));
    builder.Property(e => e.DriverReceivableId).HasConversion(uuid => uuid!.Value, value => new DriverReceivableId(value));
    builder.Property(e => e.DeliveryNoteId).HasConversion(uuid => uuid!.Value, value => new DeliveryNoteId(value));
    builder.Property(e => e.CreatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    builder.Property(e => e.UpdatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    #endregion

    builder.Property(e => e.ExpenseId).ValueGeneratedNever();
    builder.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
    builder.Property(e => e.CreatedOn).HasColumnType("datetime");
    builder.Property(e => e.Details).HasMaxLength(250);
    builder.Property(e => e.ExpenseDate).HasColumnType("datetime");
    builder.Property(e => e.UpdatedOn).HasColumnType("datetime");
  }
}
