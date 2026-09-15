using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class ProductCategoryLookupConfiguration : IEntityTypeConfiguration<ProductCategoryLookup>
{
  public void Configure(EntityTypeBuilder<ProductCategoryLookup> builder)
  {
    builder.HasKey(e => e.ProductCategoryId);
    builder.ToTable("ProductCategoryLookup");
    builder.Property(e => e.Name).HasMaxLength(100);
  }
}
