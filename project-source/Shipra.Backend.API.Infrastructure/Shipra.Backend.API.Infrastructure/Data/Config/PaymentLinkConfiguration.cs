using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.WalletAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class PaymentLinkConfiguration : IEntityTypeConfiguration<PaymentLink>
{
  public void Configure(EntityTypeBuilder<PaymentLink> builder)
  {
    builder.ToTable("PaymentLink");
    #region conversion
    builder.Property(e => e.PaymentLinkId).HasConversion(uuid => uuid!.Value, value => new PaymentLinkId(value));
    builder.Property(e => e.OrderId).HasConversion(uuid => uuid!.Value, value => new OrderId(value));
    builder.Property(e => e.PayoutId).HasConversion(uuid => uuid!.Value, value => new PayoutId(value));
    builder.Property(e => e.ClientId).HasConversion(uuid => uuid!.Value, value => new ClientId(value));
    #endregion
    builder.Property(e => e.PaymentLinkId).ValueGeneratedNever();
    builder.Property(e => e.CreatedOn).HasColumnType("datetime");
    builder.Property(e => e.PaidOn).HasColumnType("datetime");
    builder.Property(e => e.PaymentLinkPdf).HasMaxLength(500);
    builder.Property(e => e.PaymentLinkUrl).HasMaxLength(500);
    builder.Property(e => e.PaymentReleaseDate).HasColumnType("datetime");
    builder.Property(e => e.ScheduledPayoutDate).HasColumnType("datetime");
  }
}
