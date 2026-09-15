using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class ClientOrderLabelConfiguration : IEntityTypeConfiguration<ClientOrderLabel>
{
  public void Configure(EntityTypeBuilder<ClientOrderLabel> builder)
  { 
    #region convrsion
    builder.Property(e => e.ClientOrderLabelId).HasConversion(uuid => uuid!.Value, value => new ClientOrderLabelId(value));
    builder.Property(e => e.OrderId).HasConversion(uuid => uuid!.Value, value => new OrderId(value));
    builder.Property(e => e.ClientId).HasConversion(uuid => uuid!.Value, value => new ClientId(value));
    builder.Property(e => e.CreatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    builder.Property(e => e.UpdatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    #endregion

    builder.HasKey(e => e.ClientOrderLabelId).HasName("PK__ClientOr__AA6CFD1A36D63EAF");

    builder.ToTable("ClientOrderLabel");

    builder.Property(e => e.ClientOrderLabelId).ValueGeneratedNever();
    builder.Property(e => e.Active).HasDefaultValueSql("((1))");
    builder.Property(e => e.ColorCode).HasMaxLength(10);
    builder.Property(e => e.CreatedOn)
        .HasDefaultValueSql("(getutcdate())")
        .HasColumnType("datetime");
    builder.Property(e => e.LabelName).HasMaxLength(255);
    builder.Property(e => e.UpdatedOn).HasColumnType("datetime");

  }
}
