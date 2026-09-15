using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.MenuAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
  public void Configure(EntityTypeBuilder<MenuItem> builder)
  {
    builder.HasKey(e => e.MenuItemId);
    builder.ToTable("MenuItem");

    #region conversion
    builder.Property(e => e.CreatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    builder.Property(e => e.UpdatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    #endregion

    builder.Property(e => e.MenuItemName).HasMaxLength(50);
    builder.Property(e => e.MenuItemIcon).HasMaxLength(50);
    builder.Property(e => e.Description).HasMaxLength(250);
    builder.Property(e => e.RoutePath).HasMaxLength(250);
    builder.Property(e => e.TabBarTitle).HasMaxLength(50);
  }
}
