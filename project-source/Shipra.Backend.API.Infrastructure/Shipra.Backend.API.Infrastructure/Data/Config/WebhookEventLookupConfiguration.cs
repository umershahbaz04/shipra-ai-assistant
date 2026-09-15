using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.WebhookEventAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class WebhookEventLookupConfiguration : IEntityTypeConfiguration<WebhookEventLookup>
{
  public void Configure(EntityTypeBuilder<WebhookEventLookup> builder)
  {
    builder.ToTable("WebhookEventLookup");

    builder.Property(e => e.Config).HasColumnType("text");
    builder.Property(e => e.Description).HasMaxLength(500);
    builder.Property(e => e.EventName).HasMaxLength(150);
  }
}
