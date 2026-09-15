using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class ClientRolePermissionGroupConfiguration : IEntityTypeConfiguration<ClientRolePermissionGroup>
{
  public void Configure(EntityTypeBuilder<ClientRolePermissionGroup> builder)
  {
    // Pg stands for permission group
    builder.HasKey(e => e.ClientRolePgid);
    #region conversion 
    builder.Property(e => e.ClientId).HasConversion(clientId => clientId!.Value, value => new ClientId(value!));  
    #endregion

    builder.ToTable("ClientRolePermissionGroup");

    builder.Property(e => e.ClientRolePgid)
        .HasComment("Client Role Permission GroupId")
        .HasColumnName("ClientRolePGId");
  }
}

