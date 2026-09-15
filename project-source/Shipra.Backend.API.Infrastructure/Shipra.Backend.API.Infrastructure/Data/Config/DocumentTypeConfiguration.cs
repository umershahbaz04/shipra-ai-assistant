using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.DocumentAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class DocumentTypeConfiguration : IEntityTypeConfiguration<DocumentType>
{
  public void Configure(EntityTypeBuilder<DocumentType> builder)
  {
    builder.ToTable("DocumentType");

    builder.Property(e => e.Value).HasMaxLength(50);
  }
}
