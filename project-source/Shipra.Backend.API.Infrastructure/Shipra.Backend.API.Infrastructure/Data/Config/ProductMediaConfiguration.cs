using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.ProductAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class ProductMediaConfiguration : IEntityTypeConfiguration<ProductMedia>
{
  public void Configure(EntityTypeBuilder<ProductMedia> builder)
  {
    builder.ToTable("ProductMedia");
    #region convrsion
    builder.Property(e => e.ProductId).HasConversion(uuid => uuid!.Value, value => new ProductId(value)); 
    builder.Property(e => e.CreatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value)); 
    builder.Property(e => e.UpdatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value)); 
    #endregion
    builder.HasKey(x => x.ProductMediaId); 
    builder.Property(x => x.ProductId)
        .IsRequired();  
    builder.Property(x => x.CreatedOn);  
    builder.Property(x => x.Active);
  }
}
