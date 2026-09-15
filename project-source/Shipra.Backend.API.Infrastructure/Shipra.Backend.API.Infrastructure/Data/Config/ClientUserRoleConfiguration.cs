using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.ClientAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class ClientUserRoleConfiguration : IEntityTypeConfiguration<ClientUserRole>
{
  public void Configure(EntityTypeBuilder<ClientUserRole> builder)
  {
    builder.ToTable("ClientUserRole");
    #region conversion 
    builder.Property(e => e.ClientId).HasConversion(clientId => clientId!.Value, value => new ClientId(value!));
    #endregion
    builder.Property(e => e.RoleDescription).HasMaxLength(500);
    builder.Property(e => e.RoleName).HasMaxLength(50);
  }
}
