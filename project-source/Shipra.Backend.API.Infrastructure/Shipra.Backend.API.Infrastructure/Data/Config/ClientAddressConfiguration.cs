using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class ClientAddressConfiguration : IEntityTypeConfiguration<ClientAddress>
{
  public void Configure(EntityTypeBuilder<ClientAddress> builder)
  {
    builder.HasKey(e => e.ClientAddressId).HasName("PK_ClientAddressLookup");

    builder.ToTable("ClientAddress");
    #region conversion

    builder.Property(e => e.ClientAddressId).HasConversion(clientAddressId => clientAddressId!.Value, value => new ClientAddressId(value!));
    builder.Property(e => e.ClientId).HasConversion(clientId => clientId!.Value, value => new ClientId(value!));

    #endregion

    builder.Property(e => e.ClientAddressId).ValueGeneratedNever();
    builder.Property(e => e.StreetAddress).HasMaxLength(150);
    builder.Property(e => e.Zip).HasMaxLength(10);
    builder.Property(e => e.Latitude).HasColumnType("decimal(8, 6)");
    builder.Property(e => e.Longitude).HasColumnType("decimal(9, 6)");
  }
}
