using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class ProductStockConfiguration : IEntityTypeConfiguration<ProductStock>
{
  public void Configure(EntityTypeBuilder<ProductStock> builder)
  {
    builder.HasKey(e => e.ProductStockId);
    builder.ToTable("ProductStock");

    #region conversion
    builder.Property(e => e.ProductId).HasConversion(productId => productId!.Value, value => new ProductId(value!));
    builder.Property(e => e.CreatedBy).HasConversion(productId => productId!.Value, value => new EmployeeId(value!));
    builder.Property(e => e.UpdatedBy).HasConversion(productId => productId!.Value, value => new EmployeeId(value!));
    #endregion

    builder.Property(e => e.CreatedOn).HasColumnType("datetime");
    builder.Property(e => e.Price).HasColumnType("decimal(18, 2)");
    builder.Property(e => e.QuantityAvailable).HasDefaultValueSql("((0))");
    builder.Property(e => e.Sku)
        .HasMaxLength(50)
        .HasColumnName("SKU");
    builder.Property(e => e.UpdatedOn).HasColumnType("datetime");
    builder.Property(e => e.VarientOption).HasMaxLength(500);
  }
}
