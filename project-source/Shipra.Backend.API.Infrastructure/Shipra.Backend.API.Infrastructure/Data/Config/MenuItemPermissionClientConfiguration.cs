using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.UserRoleAndPermissionAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class MenuItemPermissionClientConfiguration : IEntityTypeConfiguration<MenuItemPermissionClient>
{
  public void Configure(EntityTypeBuilder<MenuItemPermissionClient> builder)
  {
    builder.HasKey(e => e.PermissionId);
    builder.ToTable("menuitemPermissionclient");

    #region conversion
    builder.Property(e => e.ClientId).HasConversion(clientId => clientId!.Value, value => new ClientId(value!));
    #endregion

    builder.Property(e => e.PermissionName).HasMaxLength(50);
    builder.Property(e => e.Description).HasMaxLength(250);
  }
}
