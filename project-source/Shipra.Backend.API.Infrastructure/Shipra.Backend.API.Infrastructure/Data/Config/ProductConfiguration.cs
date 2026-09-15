using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
  public void Configure(EntityTypeBuilder<Product> builder)
  {

    builder.HasKey(e => e.ProductId);
    builder.ToTable("Product");
    #region conversion
    builder.Property(e => e.ProductId).HasConversion(productId => productId!.Value, value => new ProductId(value!));
    builder.Property(e => e.ClientId).HasConversion(clientId => clientId!.Value, value => new ClientId(value!));
    builder.Property(e => e.CreatedBy).HasConversion(createdBy => createdBy!.Value, value => new EmployeeId(value!));
    builder.Property(e => e.UpdatedBy).HasConversion(updatedBy => updatedBy!.Value, value => new EmployeeId(value!));
    #endregion


    builder.Property(e => e.ProductId).ValueGeneratedNever();
    builder.Property(e => e.Sku)
       .HasMaxLength(20)
       .HasColumnName("SKU");
    builder.Property(e => e.ProductName).HasMaxLength(50);
    builder.Property(e => e.Price).HasColumnType("money");
    builder.Property(e => e.PurchasePrice).HasColumnType("money");
    builder.Property(e => e.Description).HasMaxLength(500);
    builder.Property(e => e.ProductCategoryId).HasColumnType("int");
    builder.Property(e => e.FeatureImage).HasMaxLength(255);
    builder.Property(e => e.Weight).HasColumnType("decimal(18, 2)");
    builder.Property(e => e.HaveOptions).HasColumnType("bool");
    builder.Property(e => e.VarientCount).HasColumnType("int");
    builder.Property(e => e.QuantityAvailable).HasColumnType("int");
    builder.Property(e => e.ClientId).HasMaxLength(36); 
    builder.Property(e => e.ProductStatusId).HasColumnType("int");
    builder.Property(e => e.TrackInventory).HasColumnType("bool");
    builder.Property(e => e.UpdatedOn).HasColumnType("datetime");
    builder.Property(e => e.CreatedOn).HasColumnType("datetime");
    //#region ProductOptions
    ////relationship with ProductOptions
    //builder.HasMany(e => e.ProductOptions)
    //           .WithOne()
    //           .HasForeignKey(p => p.ProductId);

    //builder.Metadata
    //    .FindNavigation(nameof(Product.ProductOptions))!
    //    .SetPropertyAccessMode(PropertyAccessMode.Field);
    //#endregion 

  }
}
