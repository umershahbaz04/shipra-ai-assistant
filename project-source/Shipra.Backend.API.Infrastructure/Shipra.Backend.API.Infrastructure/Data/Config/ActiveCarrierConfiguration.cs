using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class ActiveCarrierConfiguration : IEntityTypeConfiguration<ActiveCarrier>
{
  public void Configure(EntityTypeBuilder<ActiveCarrier> builder)
  {
    builder.HasKey(e => e.ActiveCarrierId);
    builder.ToTable("ActiveCarrier");
    #region convrsion
    builder.Property(e => e.ClientId).HasConversion(uuid => uuid!.Value, value => new ClientId(value));
    builder.Property(e => e.CreatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    builder.Property(e => e.UpdatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    #endregion
    builder.Property(e => e.IsActiveCarrier)
                .HasDefaultValueSql("((0))");
    builder.Property(e => e.Active)
                .HasDefaultValueSql("((1))");
    builder.Property(e => e.CreatedOn).HasColumnType("datetime");
    builder.Property(e => e.UpdatedOn).HasColumnType("datetime");
  }
}
