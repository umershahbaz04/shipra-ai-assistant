using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
internal class DriverCtssettingConfiguration : IEntityTypeConfiguration<DriverCTSSetting>
{
  public void Configure(EntityTypeBuilder<DriverCTSSetting> builder)
  {
    builder.HasKey(e => e.DriverCTSSettingId);
    #region conversion

    builder.Property(e => e.DriverCTSSettingId).HasConversion(id => id!.Value, value => new DriverCTSSettingId(value!));
    builder.Property(e => e.ClientId).HasConversion(clientId => clientId!.Value, value => new ClientId(value!));
    builder.Property(e => e.UpdatedBy).HasConversion(id => id!.Value, value => new EmployeeId(value!));
    builder.Property(e => e.CreatedBy).HasConversion(id => id!.Value, value => new EmployeeId(value!));

    #endregion

    builder.ToTable("DriverCTSSetting");

    builder.Property(e => e.DriverCTSSettingId)
        .ValueGeneratedNever()
        .HasColumnName("DriverCTSId");
    builder.Property(e => e.Active).HasDefaultValueSql("((1))");
    builder.Property(e => e.CreatedOn).HasColumnType("datetime");
    builder.Property(e => e.UpdatedOn).HasColumnType("datetime");
  }
}
