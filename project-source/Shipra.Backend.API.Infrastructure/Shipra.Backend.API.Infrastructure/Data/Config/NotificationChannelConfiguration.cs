using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.NotificationAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class NotificationChannelConfiguration : IEntityTypeConfiguration<NotificationChannel>
{
  public void Configure(EntityTypeBuilder<NotificationChannel> builder)
  {
    builder.ToTable("NotificationChannel"); 
  }
}
