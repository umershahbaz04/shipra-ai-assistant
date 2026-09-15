using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.ProductAggregate;
using Shipra.Backend.API.Core.SaleChannelProductAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class SaleChannelProductConfiguration : IEntityTypeConfiguration<SaleChannelProduct>
{
  public void Configure(EntityTypeBuilder<SaleChannelProduct> builder)
  {
    builder.ToTable("SaleChannelProduct");

    #region conversion 
    builder.Property(e => e.SaleChannelProductId).HasConversion(guid => guid!.Value, value => new SaleChannelProductId(value!));
    builder.Property(e => e.ClientId).HasConversion(guid => guid!.Value, value => new ClientId(value!));
    builder.Property(e => e.CreatedBy).HasConversion(guid => guid!.Value, value => new EmployeeId(value!));
    builder.Property(e => e.ShipraProductId).HasConversion(guid => guid!.Value, value => new ProductId(value!));
    #endregion


    builder.Property(e => e.SaleChannelProductId)
        .ValueGeneratedOnAdd()
        .HasColumnName("SaleChannelProductId");
    builder.Property(e => e.SaleChannelLookupId).HasColumnName("SaleChannelLookupId");
    builder.Property(e => e.ProductId).HasMaxLength(50);
    builder.Property(e => e.ProductJson).HasColumnType("text");
    builder.Property(e => e.ProductNo).HasMaxLength(50);

  }
}
