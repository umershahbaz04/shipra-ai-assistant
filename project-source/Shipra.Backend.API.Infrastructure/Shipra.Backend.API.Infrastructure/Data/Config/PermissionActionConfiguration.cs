using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.UserRoleAndPermissionAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class PermissionActionConfiguration : IEntityTypeConfiguration<PermissionAction>
{
  public void Configure(EntityTypeBuilder<PermissionAction> builder)
  {
    builder.ToTable("PermissionAction");

    builder.Property(e => e.ActionName).HasMaxLength(150);
    builder.Property(e => e.ControllerName).HasMaxLength(150);
  }
}
