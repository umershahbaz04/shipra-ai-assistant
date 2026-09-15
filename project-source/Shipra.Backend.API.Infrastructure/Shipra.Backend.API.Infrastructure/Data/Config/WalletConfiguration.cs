using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.WalletAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{
  public void Configure(EntityTypeBuilder<Wallet> builder)
  {
    builder.ToTable("Wallet");
    #region conversion
    builder.Property(e => e.WalletId).HasConversion(uuid => uuid!.Value, value => new WalletId(value));
    builder.Property(e => e.ClientId).HasConversion(uuid => uuid!.Value, value => new ClientId(value));
    #endregion
    builder.Property(e => e.WalletId).ValueGeneratedNever();
    builder.Property(e => e.AvailableBalance).HasColumnType("decimal(18, 2)");
    builder.Property(e => e.CurrentBalance).HasColumnType("decimal(18, 2)");
  }
}
