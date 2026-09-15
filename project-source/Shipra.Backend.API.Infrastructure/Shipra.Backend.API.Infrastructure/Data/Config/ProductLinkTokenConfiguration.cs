using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class ProductLinkTokenConfiguration : IEntityTypeConfiguration<ProductLinkToken>
{
  public void Configure(EntityTypeBuilder<ProductLinkToken> builder)
  {
    #region conversion 
    builder.Property(e => e.ProductLinkTokenId).HasConversion(id => id!.Value, value => new ProductLinkTokenId(value!));
    builder.Property(e => e.ProductId).HasConversion(id => id!.Value, value => new ProductId(value!));
    builder.Property(e => e.ClientId).HasConversion(clientId => clientId!.Value, value => new ClientId(value!));
    builder.Property(e => e.CreatedBy).HasConversion(id => id!.Value, value => new EmployeeId(value!));
    #endregion

    builder.ToTable("ProductLinkToken");

    builder.HasIndex(e => e.Token, "UQ__ProductL__1EB4F817A5ECC286").IsUnique();

    builder.Property(e => e.ProductLinkTokenId).ValueGeneratedNever();
    builder.Property(e => e.CreatedOn)
        .HasDefaultValueSql("(getdate())")
        .HasColumnType("datetime");
    builder.Property(e => e.ExpiryOn).HasColumnType("datetime");
    builder.Property(e => e.Token).HasMaxLength(20);
  }
}
