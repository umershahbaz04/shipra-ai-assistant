using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.UserRoleAndPermissionAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class UserRoleLookupConfiguration : IEntityTypeConfiguration<UserRoleLookup>
{
  public void Configure(EntityTypeBuilder<UserRoleLookup> builder)
  {
    builder.HasKey(e => e.RoleId).HasName("PK_UserRoles");

    builder.ToTable("UserRoleLookup");

    builder.Property(e => e.RoleDescription).HasMaxLength(500);
    builder.Property(e => e.RoleName).HasMaxLength(50);
  }
}
