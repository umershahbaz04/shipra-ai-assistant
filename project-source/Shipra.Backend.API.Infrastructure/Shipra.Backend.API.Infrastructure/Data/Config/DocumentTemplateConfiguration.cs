using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.DocumentAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class DocumentTemplateConfiguration : IEntityTypeConfiguration<DocumentTemplate>
{
  public void Configure(EntityTypeBuilder<DocumentTemplate> builder)
  {
    builder.ToTable("DocumentTemplate");

    builder.Property(e => e.Description).HasMaxLength(500);
    builder.Property(e => e.Name).HasMaxLength(50);
    builder.Property(e => e.Path).HasColumnType("text");
    builder.Property(e => e.SampleImageUrl).HasColumnType("text");
    builder.Property(e => e.SamplePdfUrl).HasColumnType("text");
  }
}
