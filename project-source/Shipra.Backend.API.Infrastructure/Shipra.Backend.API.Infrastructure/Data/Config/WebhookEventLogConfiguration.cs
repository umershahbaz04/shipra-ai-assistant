using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.WebhookEventAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class WebhookEventLogConfiguration : IEntityTypeConfiguration<WebhookEventLog>
{
  public void Configure(EntityTypeBuilder<WebhookEventLog> builder)
  {
    builder.ToTable("WebhookEventLog");
    #region conversion

    builder.Property(e => e.ClientId).HasConversion(clientId => clientId!.Value, value => new ClientId(value!)); 
    #endregion
    builder.Property(e => e.Active).HasDefaultValueSql("((1))");
    builder.Property(e => e.CreatedOn).HasColumnType("datetime");
    builder.Property(e => e.ErrorMessage).HasMaxLength(250);
    builder.Property(e => e.Request).HasColumnType("text");
    builder.Property(e => e.Response).HasColumnType("text");
    builder.Property(e => e.StatusName).HasMaxLength(150);
    builder.Property(e => e.UpdatedOn).HasColumnType("datetime");
  }
}
