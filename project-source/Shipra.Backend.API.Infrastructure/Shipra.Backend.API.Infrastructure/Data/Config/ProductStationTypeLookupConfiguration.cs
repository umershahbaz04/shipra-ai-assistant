using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class ProductStationTypeLookupConfiguration : IEntityTypeConfiguration<ProductStationTypeLookup>
{
    public void Configure(EntityTypeBuilder<ProductStationTypeLookup> builder)
    {
        builder.HasKey(e => e.ProductStationTypeId);
        builder.ToTable("ProductStationTypeLookup");

        builder.Property(e => e.Name).HasMaxLength(100).IsRequired();
    }
}
