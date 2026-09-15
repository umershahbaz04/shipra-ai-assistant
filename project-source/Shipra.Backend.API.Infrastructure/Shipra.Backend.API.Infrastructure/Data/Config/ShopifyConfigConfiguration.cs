using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.ShopifyAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class ShopifyConfigConfiguration : IEntityTypeConfiguration<ShopifyConfig>
{
  public void Configure(EntityTypeBuilder<ShopifyConfig> builder)
  {
    builder.ToTable("ShopifyConfig");

    #region conversion 
    builder.Property(e => e.ClientId).HasConversion(guid => guid!.Value, value => new ClientId(value!));
    builder.Property(e => e.CreatedBy).HasConversion(guid => guid!.Value, value => new EmployeeId(value!));
    builder.Property(e => e.UpdatedBy).HasConversion(guid => guid!.Value, value => new EmployeeId(value!));
    #endregion


    builder.Property(e => e.ShopifyConfigId)
        .ValueGeneratedOnAdd()
        .HasColumnName("ShopifyConfigId");
    builder.Property(e => e.AccessToken).HasMaxLength(150);
    builder.Property(e => e.RefreshToken).HasMaxLength(150);
    builder.Property(e => e.ShopDomain).HasMaxLength(250);
    builder.Property(e => e.CreatedOn).HasColumnType("datetime");
    builder.Property(e => e.UpdatedOn).HasColumnType("datetime");
    builder.Property(e => e.Active).HasDefaultValueSql("((1))");

  }
}
