using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.CarrierAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class CarrierLocationConfiguration : IEntityTypeConfiguration<CarrierLocation>
{
  public void Configure(EntityTypeBuilder<CarrierLocation> builder)
  {
    builder.HasKey(e => e.CarrierLocationId).HasName("PK_CarrierLocationService"); 
    builder.ToTable("CarrierLocation");
  }
}
