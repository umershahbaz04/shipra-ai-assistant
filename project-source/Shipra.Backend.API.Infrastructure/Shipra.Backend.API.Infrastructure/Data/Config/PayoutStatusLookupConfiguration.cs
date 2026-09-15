using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.WalletAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class PayoutStatusLookupConfiguration : IEntityTypeConfiguration<PayoutStatusLookup>
{
  public void Configure(EntityTypeBuilder<PayoutStatusLookup> builder)
  {
    builder.HasKey(e => e.PayoutStatusId);

    builder.ToTable("PayoutStatusLookup");

    builder.Property(e => e.StatusName).HasMaxLength(20);
  }
}
