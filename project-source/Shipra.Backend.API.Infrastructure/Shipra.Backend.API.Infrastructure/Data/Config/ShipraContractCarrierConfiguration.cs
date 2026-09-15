using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.CarrierAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class ShipraContractCarrierConfiguration : IEntityTypeConfiguration<ShipraContractCarrier>
{
  public void Configure(EntityTypeBuilder<ShipraContractCarrier> builder)
  {
    builder.ToTable("ShipraContractCarrier");

    builder.Property(e => e.ShipraContractCarrierId); 
    builder.Property(e => e.FlatRate).HasColumnType("decimal(18, 2)");
  }
}
