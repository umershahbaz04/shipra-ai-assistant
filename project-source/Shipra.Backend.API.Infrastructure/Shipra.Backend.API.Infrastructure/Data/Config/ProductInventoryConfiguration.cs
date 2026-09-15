//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using Shipra.Backend.API.Core.ProductAggregate;

//namespace Shipra.Backend.API.Infrastructure.Data.Config;
//public class ProductInventoryConfiguration : IEntityTypeConfiguration<ProductInventory>
//{
//  public void Configure(EntityTypeBuilder<ProductInventory> builder)
//  {
//    builder.HasKey(e => e.ProductInventoryId);
//    builder.ToTable("ProductInventory");
     
//    #region conversion
//    builder.Property(e => e.ProductInventoryId).HasConversion(productOptionId => productOptionId!.Value, value => new ProductInventoryId(value));
//    builder.Property(e => e.ProductId).HasConversion(productId => productId!.Value, value => new ProductId(value!));
//    #endregion

//    builder.Property(e => e.ProductInventoryId).ValueGeneratedNever();
//    builder.Property(e => e.CreatedBy).HasMaxLength(36);
//    builder.Property(e => e.CreatedOn).HasColumnType("datetime");
//    builder.Property(e => e.LastPurchase).HasColumnType("datetime");
//    builder.Property(e => e.UpdatedBy).HasMaxLength(36);
//    builder.Property(e => e.UpdatedOn).HasColumnType("datetime");
//  }
//}
