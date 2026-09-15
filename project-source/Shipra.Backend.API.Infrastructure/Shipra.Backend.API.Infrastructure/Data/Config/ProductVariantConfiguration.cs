using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
  public void Configure(EntityTypeBuilder<ProductVariant> builder)
  {
      builder.HasKey(e => e.ProductVariantId);
      builder.ToTable("ProductVariant");

      #region conversion
      builder.Property(e => e.ProductId).HasConversion(productId => productId!.Value, value => new ProductId(value!));
      builder.Property(e => e.ClientId).HasConversion(clientId => clientId!.Value, value => new ClientId(value!));
      builder.Property(e => e.CreatedBy).HasConversion(createdBy => createdBy!.Value, value => new EmployeeId(value!));
      builder.Property(e => e.UpdatedBy).HasConversion(updatedBy => updatedBy!.Value, value => new EmployeeId(value!));
      #endregion

      builder.Property(e => e.SKU).HasMaxLength(100).IsRequired();
      builder.Property(e => e.Barcode).HasMaxLength(100);
      builder.Property(e => e.Price).HasColumnType("decimal(18,2)");
      builder.Property(e => e.PurchasePrice).HasColumnType("decimal(18,2)");
      builder.Property(e => e.Weight).HasColumnType("decimal(18,2)");
      builder.Property(e => e.Length).HasColumnType("decimal(18,2)");
      builder.Property(e => e.Width).HasColumnType("decimal(18,2)");
      builder.Property(e => e.Height).HasColumnType("decimal(18,2)"); 
  }
}
