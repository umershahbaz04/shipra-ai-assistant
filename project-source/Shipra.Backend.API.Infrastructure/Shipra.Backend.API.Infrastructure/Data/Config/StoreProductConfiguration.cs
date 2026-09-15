using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.ProductAggregate;
using Shipra.Backend.API.Core.StoresAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class StoreProductConfiguration : IEntityTypeConfiguration<StoreProduct>
{
  public void Configure(EntityTypeBuilder<StoreProduct> builder)
  {
    #region convrsion
    builder.Property(e => e.StoreProductId).HasConversion(uuid => uuid!.Value, value => new StoreProductId(value));
    builder.Property(e => e.ProductId).HasConversion(uuid => uuid!.Value, value => new ProductId(value));
    builder.Property(e => e.CreatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    builder.Property(e => e.UpdatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    #endregion

    builder.ToTable("StoreProduct");
    builder.HasKey(x => x.StoreProductId);
    builder.Property(x => x.StoreProductId).ValueGeneratedOnAdd();
  }
}
