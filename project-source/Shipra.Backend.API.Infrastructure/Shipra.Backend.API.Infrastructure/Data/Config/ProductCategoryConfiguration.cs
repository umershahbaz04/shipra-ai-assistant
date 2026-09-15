using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
{
  public void Configure(EntityTypeBuilder<ProductCategory> builder)
  {
    builder.ToTable("ProductCategory");
    builder.Property(e => e.CategoryName).HasMaxLength(50);
    builder.Property(e => e.CreatedOn).HasColumnType("datetime");
    builder.Property(e => e.UpdatedOn).HasColumnType("datetime");

    #region conversion
    builder.Property(e => e.ClientId).HasConversion(orderId => orderId!.Value, value => new ClientId(value!));

    builder.Property(e => e.CreatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    builder.Property(e => e.UpdatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    #endregion

  }
}
