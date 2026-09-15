using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class ProductStockHistoryConfiguration : IEntityTypeConfiguration<ProductStockHistory>
{
  public void Configure(EntityTypeBuilder<ProductStockHistory> builder)
  {
    builder.HasKey(e => e.ProductStockHistorytId);

    builder.ToTable("ProductStockHistory");
    #region conversion 
    builder.Property(e => e.CreatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value)); 
    #endregion
    builder.Property(e => e.Comment).HasMaxLength(250);
    builder.Property(e => e.CreatedOn).HasColumnType("datetime");
  }
}
