using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.WalletAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class PayoutConfiguration : IEntityTypeConfiguration<Payout>
{
  public void Configure(EntityTypeBuilder<Payout> builder)
  {
    builder.ToTable("Payout");
    #region conversion
    builder.Property(e => e.PayoutId).HasConversion(uuid => uuid!.Value, value => new PayoutId(value));
    builder.Property(e => e.ClientId).HasConversion(uuid => uuid!.Value, value => new ClientId(value));
    #endregion
    builder.Property(e => e.PayoutId).ValueGeneratedNever();
    builder.Property(e => e.AccountTitle).HasMaxLength(50);
    builder.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
    builder.Property(e => e.BankName).HasMaxLength(255);
    builder.Property(e => e.BranchName).HasMaxLength(50);
    builder.Property(e => e.CreatedOn).HasColumnType("datetime"); 
    builder.Property(e => e.Iban)
        .HasMaxLength(40)
        .HasColumnName("IBAN");
    builder.Property(e => e.PayoutRef).HasMaxLength(50);
    builder.Property(e => e.SwiftCode).HasMaxLength(50);
    builder.Property(e => e.TransactionRef).HasMaxLength(50);
  }
}
