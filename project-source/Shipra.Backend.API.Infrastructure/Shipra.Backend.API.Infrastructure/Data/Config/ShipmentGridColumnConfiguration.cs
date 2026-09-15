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
public class ShipmentGridColumnConfiguration : IEntityTypeConfiguration<ShipmentGridColumn>
{
  public void Configure(EntityTypeBuilder<ShipmentGridColumn> builder)
  {
    builder.ToTable("ShipmentGridColumn");
    #region conversion 
    builder.Property(e => e.ClientId).HasConversion(guid => guid!.Value, value => new ClientId(value!));
    builder.Property(e => e.CreatedBy).HasConversion(guid => guid!.Value, value => new EmployeeId(value!));
    builder.Property(e => e.UpdatedBy).HasConversion(guid => guid!.Value, value => new EmployeeId(value!));
    #endregion

    builder.Property(e => e.ColumnName).HasMaxLength(100);
    builder.Property(e => e.CreatedOn).HasColumnType("datetime");
    builder.Property(e => e.UpdatedOn).HasColumnType("datetime");
  }
}
