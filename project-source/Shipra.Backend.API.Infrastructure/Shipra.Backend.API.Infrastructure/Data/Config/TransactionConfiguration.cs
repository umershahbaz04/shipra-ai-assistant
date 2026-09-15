using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.WalletAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
  public void Configure(EntityTypeBuilder<Transaction> builder)
  {
    builder.ToTable("Transaction");
    #region conversion
    builder.Property(e => e.TransactionId).HasConversion(uuid => uuid!.Value, value => new TransactionId(value));
    builder.Property(e => e.WalletId).HasConversion(uuid => uuid!.Value, value => new WalletId(value));
    builder.Property(e => e.ClientId).HasConversion(uuid => uuid!.Value, value => new ClientId(value));
    #endregion
    builder.Property(e => e.TransactionId).ValueGeneratedNever();
    builder.Property(e => e.CreatedOn).HasColumnType("datetime");
    builder.Property(e => e.Description).HasMaxLength(500);
    builder.Property(e => e.TransactionNo).HasMaxLength(20);
  }
}
