using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.NotificationAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class NotificationEventConfiguration : IEntityTypeConfiguration<NotificationEvent>
{
  public void Configure(EntityTypeBuilder<NotificationEvent> builder)
  { 
    builder.ToTable("NotificationEvent");

    builder.Property(e => e.EventName).HasMaxLength(150);
  }
}
