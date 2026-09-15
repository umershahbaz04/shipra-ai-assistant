using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class PaymentMethodLookupConfiguration : IEntityTypeConfiguration<PaymentMethodLookup>
{
  public void Configure(EntityTypeBuilder<PaymentMethodLookup> builder)
  {
    builder.HasKey(e => e.PaymentMethodId).HasName("PK_PaymentMethod");

    builder.ToTable("PaymentMethodLookup");

    builder.Property(e => e.PMName)
        .HasMaxLength(50)
        .HasColumnName("PMName");
  }
}
