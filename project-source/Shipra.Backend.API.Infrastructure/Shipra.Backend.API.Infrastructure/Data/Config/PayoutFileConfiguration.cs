using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.WalletAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class PayoutFileConfiguration : IEntityTypeConfiguration<PayoutFile>
{
  public void Configure(EntityTypeBuilder<PayoutFile> builder)
  {
    #region conversions 
    builder.Property(e => e.PayoutId).HasConversion(uuid => uuid!.Value, value => new PayoutId(value));
    builder.Property(e => e.PayoutFileId).HasConversion(uuid => uuid!.Value, value => new PayoutFileId(value));
    #endregion

    builder.ToTable("PayoutFile");

    builder.Property(e => e.PayoutFileId).ValueGeneratedNever();
    builder.Property(e => e.FilePath).HasMaxLength(500);
  }
}
