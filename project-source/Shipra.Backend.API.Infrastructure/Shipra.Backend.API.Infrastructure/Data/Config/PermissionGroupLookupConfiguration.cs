using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.UserRoleAndPermissionAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class PermissionGroupLookupConfiguration : IEntityTypeConfiguration<PermissionGroupLookup>
{
  public void Configure(EntityTypeBuilder<PermissionGroupLookup> builder)
  {
    builder.HasKey(e => e.PermissionGroupId);
    builder.ToTable("PermissionGroupLookup");

    builder.Property(e => e.GroupName).HasMaxLength(150);
    builder.Property(e => e.GroupParent).HasMaxLength(150);
  }
}
