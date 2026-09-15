using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class ProductOptionConfiguration : IEntityTypeConfiguration<ProductOption>
{
  public void Configure(EntityTypeBuilder<ProductOption> builder)
  {
    builder.HasKey(e => e.ProductOptionsId);
    #region conversion
    builder.Property(e => e.ProductOptionsId).HasConversion(productOptionId => productOptionId!.Value, value => new ProductOptionsId(value));
    builder.Property(e => e.ProductId).HasConversion(productId => productId!.Value, value => new ProductId(value!));

    #endregion
    builder.Property(e => e.ProductOptionsId).ValueGeneratedNever();
    builder.Property(e => e.IsDeleted).HasDefaultValueSql("((0))"); 
    builder.Property(e => e.OptionValue).HasMaxLength(50);


    //builder.HasOne(d => d.Product).WithMany(p => p.ProductOptions)
    //    .HasForeignKey(d => d.ProductId)
    //    .HasConstraintName("FK_ProductOptions_Product");

  }
}
