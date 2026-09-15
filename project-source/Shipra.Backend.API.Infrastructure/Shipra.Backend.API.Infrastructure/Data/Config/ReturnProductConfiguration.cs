using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ProductAggregate;
using Shipra.Backend.API.Core.ReturnAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class ReturnProductConfiguration : IEntityTypeConfiguration<ReturnProduct>
{
  public void Configure(EntityTypeBuilder<ReturnProduct> builder)
  {
    builder.ToTable("ReturnProduct");
    #region convrsion
    builder.Property(e => e.ReturnId).HasConversion(uuid => uuid!.Value, value => new ReturnId(value)); 
    builder.Property(e => e.ReturnProductId).HasConversion(uuid => uuid!.Value, value => new ReturnProductId(value)); 
    builder.Property(e => e.ProductId).HasConversion(uuid => uuid!.Value, value => new ProductId(value)); 
    #endregion
    builder.Property(e => e.ReturnProductId).ValueGeneratedNever();
    builder.Property(e => e.ItemValue)
        .HasDefaultValueSql("((0))")
        .HasColumnType("decimal(18, 2)");
  }

}
