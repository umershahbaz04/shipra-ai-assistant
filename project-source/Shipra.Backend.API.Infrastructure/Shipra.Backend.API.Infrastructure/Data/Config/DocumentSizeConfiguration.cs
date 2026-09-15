using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.DocumentAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class DocumentSizeConfiguration : IEntityTypeConfiguration<DocumentSize>
{
  public void Configure(EntityTypeBuilder<DocumentSize> builder)
  {
    builder.ToTable("DocumentSize");

    builder.Property(e => e.Name).HasMaxLength(50);
  }
}
