using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.CatalougeAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
internal class CatalogueDatabaseConfiguration : IEntityTypeConfiguration<CatalogueDatabase>
{
  public void Configure(EntityTypeBuilder<CatalogueDatabase> builder)
  {
    builder.HasKey(e => e.DatabaseId);

    builder.ToTable("CatalogueDatabase");

    builder.Property(e => e.ConnectionString).HasColumnType("text");
    builder.Property(e => e.IpAddress).HasMaxLength(50);
    builder.Property(e => e.QuantityUsed).HasDefaultValueSql("((0))");
  }
}
