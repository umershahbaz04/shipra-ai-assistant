using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.ClientAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class ShipraContractClientCarrierConfiguration : IEntityTypeConfiguration<ShipraContractClientCarrier>
{
  public void Configure(EntityTypeBuilder<ShipraContractClientCarrier> builder)
  {
    builder.ToTable("ShipraContractClientCarrier");

    #region convrsion
    builder.Property(e => e.ClientId).HasConversion(uuid => uuid!.Value, value => new ClientId(value)); 
    #endregion
  }
}
