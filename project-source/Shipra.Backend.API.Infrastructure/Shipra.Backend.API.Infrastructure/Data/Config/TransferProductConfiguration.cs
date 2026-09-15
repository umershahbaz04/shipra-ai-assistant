using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class TransferProductConfiguration : IEntityTypeConfiguration<TransferProduct>
{
  public void Configure(EntityTypeBuilder<TransferProduct> builder)
  {
    builder.HasKey(e => e.TransferProductsId);
    builder.ToTable("TransferProduct");


    #region conversion 
    builder.Property(e => e.ProductStaionTransferId).HasConversion(uuid => uuid!.Value, value => new ProductStaionTransferId(value));
    builder.Property(e => e.ProductId).HasConversion(uuid => uuid!.Value, value => new ProductId(value));
    #endregion

    builder.Property(e => e.ProductSku)
        .HasMaxLength(20)
        .HasColumnName("ProductSKU");
  }
}
