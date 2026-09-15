using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
internal class LookupProductStockStatusConfiguration : IEntityTypeConfiguration<LookupProductStockStatus>
{
  public void Configure(EntityTypeBuilder<LookupProductStockStatus> builder)
  {
    builder.HasKey(e => e.ProductStockStatusId); 
    builder.ToTable("LookupProductStockStatus"); 
    builder.Property(e => e.StatusName).HasMaxLength(50);
  }
}
