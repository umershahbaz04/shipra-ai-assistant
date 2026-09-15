using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class OrderNoteConfiguration : IEntityTypeConfiguration<OrderNote>
{
  public void Configure(EntityTypeBuilder<OrderNote> builder)
  {
    #region conversion

    builder.Property(e => e.OrderNoteId).HasConversion(orderId => orderId!.Value, value => new OrderNoteId(value!));
    builder.Property(e => e.OrderId).HasConversion(orderId => orderId!.Value, value => new OrderId(value!));
    builder.Property(e => e.CreatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    builder.Property(e => e.UpdatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    #endregion

    builder.ToTable("OrderNote");

    builder.Property(e => e.OrderNoteId).ValueGeneratedNever();
    builder.Property(e => e.Active).HasDefaultValueSql("((1))");
    builder.Property(e => e.CreatedOn).HasColumnType("datetime");
    builder.Property(e => e.NoteDescription).HasMaxLength(250);
    builder.Property(e => e.UpdatedOn).HasColumnType("datetime");
  }
}
