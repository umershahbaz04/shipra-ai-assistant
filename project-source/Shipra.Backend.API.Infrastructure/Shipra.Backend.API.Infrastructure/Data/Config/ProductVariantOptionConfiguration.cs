using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class ProductVariantOptionConfiguration : IEntityTypeConfiguration<ProductVariantOption>
{
  public void Configure(EntityTypeBuilder<ProductVariantOption> builder)
  {
      builder.HasKey(e => e.ProductVariantOptionId);
      builder.ToTable("ProductVariantOption");

      builder.Property(e => e.ProductVariantId).IsRequired();
      builder.Property(e => e.ProductOptionsId).IsRequired();
      builder.Property(e => e.CreatedOn).HasColumnType("datetime").IsRequired().HasDefaultValueSql("GETDATE()");
  }
}
