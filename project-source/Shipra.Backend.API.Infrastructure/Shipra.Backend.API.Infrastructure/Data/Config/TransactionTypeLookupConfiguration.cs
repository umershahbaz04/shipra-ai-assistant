using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.WalletAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class TransactionTypeLookupConfiguration : IEntityTypeConfiguration<TransactionTypeLookup>
{
  public void Configure(EntityTypeBuilder<TransactionTypeLookup> builder)
  {
    builder.HasKey(e => e.TransactionTypeId);

    builder.ToTable("TransactionTypeLookup");

    builder.Property(e => e.TransactionName).HasMaxLength(50);
  }
}
