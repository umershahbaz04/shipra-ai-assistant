using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.NotificationAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class NotificationConfigConfiguration : IEntityTypeConfiguration<NotificationConfig>
{
  public void Configure(EntityTypeBuilder<NotificationConfig> builder)
  {
    builder.ToTable("NotificationConfig");
    #region conversion
    builder.Property(e => e.NotificationConfigId).HasConversion(uuid => uuid!.Value, value => new NotificationConfigId(value));
    builder.Property(e => e.ClientId).HasConversion(uuid => uuid!.Value, value => new ClientId(value));
    builder.Property(e => e.CreatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    builder.Property(e => e.UpdatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    #endregion
    builder.Property(e => e.Active).HasDefaultValueSql("((1))");
    builder.Property(e => e.CreatedOn).HasColumnType("datetime"); 
    builder.Property(e => e.UpdateOn).HasColumnType("datetime");
  }
}
