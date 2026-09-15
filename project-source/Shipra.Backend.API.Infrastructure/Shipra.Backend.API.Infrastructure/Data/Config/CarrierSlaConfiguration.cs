using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.CarrierAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class CarrierSlaConfiguration : IEntityTypeConfiguration<CarrierSla>
{
  public void Configure(EntityTypeBuilder<CarrierSla> builder)
  {
    builder.ToTable("CarrierSLA");

    builder.Property(e => e.DeliveryMethod).HasMaxLength(250);
    builder.Property(e => e.DeliveryTime).HasMaxLength(50);
    builder.Property(e => e.DropOffMethod).HasMaxLength(250);
  }
}
