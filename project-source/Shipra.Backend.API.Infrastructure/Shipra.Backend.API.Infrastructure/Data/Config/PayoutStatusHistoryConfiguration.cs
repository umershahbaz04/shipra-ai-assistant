using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.WalletAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class PayoutStatusHistoryConfiguration : IEntityTypeConfiguration<PayoutStatusHistory>
{
  public void Configure(EntityTypeBuilder<PayoutStatusHistory> builder)
  {
    builder.ToTable("PayoutStatusHistory");
    #region conversion
    builder.Property(e => e.PayoutStatusHistoryId).HasConversion(uuid => uuid!.Value, value => new PayoutStatusHistoryId(value));
    builder.Property(e => e.PayoutId).HasConversion(uuid => uuid!.Value, value => new PayoutId(value));
    #endregion
    builder.Property(e => e.PayoutStatusHistoryId).ValueGeneratedNever();
    builder.Property(e => e.Comment).HasMaxLength(50);
    builder.Property(e => e.CreatedBy).HasMaxLength(50);
    builder.Property(e => e.CreatedOn).HasColumnType("datetime");
  }
}
