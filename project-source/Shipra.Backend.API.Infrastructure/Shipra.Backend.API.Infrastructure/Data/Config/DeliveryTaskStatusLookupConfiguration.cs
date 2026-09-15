using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.DeliveryTaskAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class DeliveryTaskStatusLookupConfiguration : IEntityTypeConfiguration<DeliveryTaskStatusLookup>
{
  public void Configure(EntityTypeBuilder<DeliveryTaskStatusLookup> builder)
  {
    builder.HasKey(e => e.DeliveryTaskStatusId);
    builder.ToTable("DeliveryTaskStatusLookup");
    builder.Property(e => e.DeliveryTaskStatus).HasMaxLength(50);
  }
}
