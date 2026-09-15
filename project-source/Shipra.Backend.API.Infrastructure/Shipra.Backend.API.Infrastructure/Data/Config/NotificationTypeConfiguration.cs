using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.NotificationAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class NotificationTypeConfiguration : IEntityTypeConfiguration<NotificationType>
{
  public void Configure(EntityTypeBuilder<NotificationType> builder)
  {
    builder.ToTable("NotificationType");

    builder.Property(e => e.TypeName).HasMaxLength(50);
  }
}
