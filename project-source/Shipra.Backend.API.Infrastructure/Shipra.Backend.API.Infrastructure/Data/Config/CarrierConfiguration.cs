using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class CarrierConfiguration : IEntityTypeConfiguration<Carrier>
{
  public void Configure(EntityTypeBuilder<Carrier> builder)
  {
    builder.HasKey(e => e.CarrierId);
    #region conversion
    builder.Property(e => e.CreatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    builder.Property(e => e.UpdatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    #endregion
    builder.ToTable("Carrier");

    builder.Property(e => e.Active).HasDefaultValueSql("((1))");
    builder.Property(e => e.IsClientCarrier).HasDefaultValueSql("((0))");
    builder.Property(e => e.CarrierImage).HasMaxLength(255);
    builder.Property(e => e.CarrierWebsite).HasMaxLength(255);
    builder.Property(e => e.Config).HasColumnType("text");
    builder.Property(e => e.InputRequiredConfig).HasColumnType("text");
    builder.Property(e => e.CreatedOn).HasColumnType("datetime");
    builder.Property(e => e.Name).HasMaxLength(250);
    builder.Property(e => e.UpdateOn).HasColumnType("datetime");
  }
}
