using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class ClientOrderLabelLookupConfiguration : IEntityTypeConfiguration<ClientOrderLabelLookup>
{
  public void Configure(EntityTypeBuilder<ClientOrderLabelLookup> builder)
  { 
    #region convrsion
    builder.Property(e => e.ClientId).HasConversion(uuid => uuid!.Value, value => new ClientId(value));
    builder.Property(e => e.CreatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    builder.Property(e => e.UpdatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    #endregion

    builder.HasKey(e => e.ClientOrderLabelLookupId).HasName("PK__ClientOr__94D94407618D325A");

    builder.ToTable("ClientOrderLabelLookup");

    builder.Property(e => e.Active).HasDefaultValueSql("((1))");
    builder.Property(e => e.ColorCode).HasMaxLength(10);
    builder.Property(e => e.CreatedOn)
        .HasDefaultValueSql("(getutcdate())")
        .HasColumnType("datetime");
    builder.Property(e => e.LabelName).HasMaxLength(255);
    builder.Property(e => e.UpdatedOn).HasColumnType("datetime");
  }
}
