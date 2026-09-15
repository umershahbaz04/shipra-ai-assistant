using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.MenuAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class MenuConfiguration : IEntityTypeConfiguration<Menu>
{
  public void Configure(EntityTypeBuilder<Menu> builder)
  {
    builder.HasKey(e => e.MenuId);
    builder.ToTable("Menu");

    #region conversion
    builder.Property(e => e.CreatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    builder.Property(e => e.UpdatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    #endregion

    builder.Property(e => e.MenuName).HasMaxLength(50);
    builder.Property(e => e.MenuIcon).HasMaxLength(50);
    builder.Property(e => e.Description).HasMaxLength(250);
    builder.Property(e => e.TabBarTitle).HasMaxLength(50);
    builder.Property(e => e.RoutePath).HasMaxLength(250);
  }
}
