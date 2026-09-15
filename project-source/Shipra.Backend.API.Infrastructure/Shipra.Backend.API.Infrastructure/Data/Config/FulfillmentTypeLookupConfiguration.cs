using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.StoresAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class FulfillmentTypeLookupConfiguration : IEntityTypeConfiguration<FulfillmentTypeLookup>
{
    public void Configure(EntityTypeBuilder<FulfillmentTypeLookup> builder)
    {
        builder.HasKey(e => e.FulfillmentTypeId);
        builder.ToTable("FulfillmentTypeLookup");

        builder.Property(e => e.Name).HasMaxLength(100).IsRequired();
    }
}
