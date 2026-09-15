using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.UserRoleAndPermissionAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class RolePermissionGroupDefaultConfiguration : IEntityTypeConfiguration<RolePermissionGroupDefault>
{
  public void Configure(EntityTypeBuilder<RolePermissionGroupDefault> builder)
  {
    builder.HasKey(e => e.RolePermissionGroupId);

    builder.ToTable("RolePermissionGroupDefault");
     
  }
}
