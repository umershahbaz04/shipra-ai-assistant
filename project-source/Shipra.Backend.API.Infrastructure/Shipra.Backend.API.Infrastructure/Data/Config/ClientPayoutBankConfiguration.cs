using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.WalletAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class ClientPayoutBankConfiguration : IEntityTypeConfiguration<ClientPayoutBank>
{
  public void Configure(EntityTypeBuilder<ClientPayoutBank> builder)
  {
    builder.ToTable("ClientPayoutBank");
    #region conversion
    builder.Property(e => e.ClientPayoutBankId).HasConversion(uuid => uuid!.Value, value => new ClientPayoutBankId(value));
    builder.Property(e => e.ClientId).HasConversion(uuid => uuid!.Value, value => new ClientId(value));
    #endregion
    builder.Property(e => e.ClientPayoutBankId).ValueGeneratedNever();
    builder.Property(e => e.AccountTitle).HasMaxLength(50);
    builder.Property(e => e.BankName).HasMaxLength(255);
    builder.Property(e => e.BranchName).HasMaxLength(50);
    builder.Property(e => e.Iban)
        .HasMaxLength(40)
        .HasColumnName("IBAN");
    builder.Property(e => e.SwiftCode).HasMaxLength(50);
  }
}
