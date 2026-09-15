using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.CarrierAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class DeliveryServiceConfiguration : IEntityTypeConfiguration<DeliveryService>
{
  public void Configure(EntityTypeBuilder<DeliveryService> builder)
  {
    builder.ToTable("DeliveryService");

    builder.Property(e => e.ServiceLogo).HasMaxLength(300);
    builder.Property(e => e.ServiceName).HasMaxLength(255);
  }
}
