using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.CommonAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class WhatsAppCategoryTypeConfiguration : IEntityTypeConfiguration<WhatsAppCategoryType>
{
  public void Configure(EntityTypeBuilder<WhatsAppCategoryType> builder)
  {
    builder.HasKey(e => e.WhatsAppCategoryTypeId);
    builder.ToTable("WhatsAppCategoryType");
    builder.Property(e => e.Name).HasMaxLength(50);
  }
}
