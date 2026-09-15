using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.SettingOperationDashboardAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class ShipmentGridClientSettingConfiguration : IEntityTypeConfiguration<ShipmentGridClientSetting>
{
  public void Configure(EntityTypeBuilder<ShipmentGridClientSetting> builder)
  {
    builder.HasKey(e => e.ShipmentGridClientSettingId).HasName("PK_ShipmentSettingClient");

    builder.ToTable("ShipmentGridClientSetting");
    #region conversion 
    builder.Property(e => e.ShipmentGridClientSettingId).HasConversion(guid => guid!.Value, value => new ShipmentGridClientSettingId(value!));
    builder.Property(e => e.ClientId).HasConversion(guid => guid!.Value, value => new ClientId(value!));
    builder.Property(e => e.CreatedBy).HasConversion(guid => guid!.Value, value => new EmployeeId(value!));
    builder.Property(e => e.UpdatedBy).HasConversion(guid => guid!.Value, value => new EmployeeId(value!));
    #endregion
    builder.Property(e => e.ShipmentGridClientSettingId).ValueGeneratedNever();
    builder.Property(e => e.Active).HasDefaultValueSql("((1))");
    builder.Property(e => e.CreatedOn).HasColumnType("datetime");
    builder.Property(e => e.DashboardStatusValue).HasMaxLength(250);
    builder.Property(e => e.UpdatedOn).HasColumnType("datetime");
  }
}
