//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using Shipra.Backend.API.Core.ProductAggregate;

//namespace Shipra.Backend.API.Infrastructure.Data.Config;
//internal class ProductFileConfiguration : IEntityTypeConfiguration<ProductFile>
//{
//  public void Configure(EntityTypeBuilder<ProductFile> builder)
//  {
//    builder.HasKey(e => e.ProductFileId);
//    builder.ToTable("ProductFile");

//    builder.Property(e => e.ProductFileId).HasConversion(productOptionId => productOptionId!.Value, value => new ProductFileId(value));
//    builder.Property(e => e.ProductId).HasConversion(productId => productId!.Value, value => new ProductId(value!));

//    builder.Property(e => e.ProductFileId).ValueGeneratedNever();
//    builder.Property(e => e.CreatedBy).HasMaxLength(36);
//    builder.Property(e => e.CreatedOn).HasColumnType("datetime");
//    builder.Property(e => e.Path).HasMaxLength(255);
//    builder.Property(e => e.Type).HasMaxLength(50);
//    builder.Property(e => e.UpdatedBy).HasMaxLength(36);
//    builder.Property(e => e.UpdatedOn).HasColumnType("datetime");
    
//    //builder.HasOne(d => d.Product).WithMany(p => p.ProductFiles)
//    //    .HasForeignKey(d => d.ProductId)
//    //    .HasConstraintName("FK_ProductFile_Product");
//  }
//}
