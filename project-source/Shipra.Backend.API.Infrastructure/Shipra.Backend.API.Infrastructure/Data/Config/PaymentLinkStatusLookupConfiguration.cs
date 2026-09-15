using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.WalletAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class PaymentLinkStatusLookupConfiguration : IEntityTypeConfiguration<PaymentLinkStatusLookup>
{
  public void Configure(EntityTypeBuilder<PaymentLinkStatusLookup> builder)
  {
    builder.HasKey(e => e.PaymentLinkStatusId);

    builder.ToTable("PaymentLinkStatusLookup");

    builder.Property(e => e.StatusName)
        .HasMaxLength(10)
        .IsFixedLength();
  }
}
