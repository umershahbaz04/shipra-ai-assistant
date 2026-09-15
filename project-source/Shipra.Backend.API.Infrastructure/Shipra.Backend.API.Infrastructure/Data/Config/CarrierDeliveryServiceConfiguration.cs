using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.CarrierAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class CarrierDeliveryServiceConfiguration : IEntityTypeConfiguration<CarrierDeliveryService>
{
  public void Configure(EntityTypeBuilder<CarrierDeliveryService> builder)
  {
    builder.HasKey(e => e.CarrierDeliveryServiceId);
    builder.ToTable("CarrierDeliveryService");

  }
}
