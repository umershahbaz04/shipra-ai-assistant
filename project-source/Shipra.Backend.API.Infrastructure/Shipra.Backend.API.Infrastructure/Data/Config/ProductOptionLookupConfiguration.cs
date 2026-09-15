using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.CommonAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class ProductOptionLookupConfiguration : IEntityTypeConfiguration<ProductOptionLookup>
{
  public void Configure(EntityTypeBuilder<ProductOptionLookup> builder)
  {
    builder.HasKey(e => e.ProductOptionId);
    builder.ToTable("ProductOptionLookup");
    builder.Property(e => e.Name).HasMaxLength(50);
  }
}
