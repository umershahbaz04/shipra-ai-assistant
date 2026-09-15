using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
  public void Configure(EntityTypeBuilder<Client> builder)
  {
    builder.HasKey(e => e.ClientId);
    builder.ToTable("Client");
    #region conversion

    builder.Property(e => e.ClientId).HasConversion(clientId => clientId!.Value, value => new ClientId(value!));
    builder.Property(e => e.UpdatedBy).HasConversion(id => id!.Value, value => new EmployeeId(value!));
    builder.Property(e => e.CreatedBy).HasConversion(id => id!.Value, value => new EmployeeId(value!));

    #endregion

    builder.HasIndex(e => e.ClientCode, "UQ_ClientCode").IsUnique();

    builder.Property(e => e.ClientId).ValueGeneratedNever();
    builder.Property(e => e.Active).HasDefaultValueSql("((1))"); 
    builder.Property(e => e.DefaultStoreId).HasMaxLength(36);
    builder.Property(e => e.DefaultCarrierId).HasMaxLength(36);
    builder.Property(e => e.ClientCode).HasMaxLength(20);
    builder.Property(e => e.ClientCompanyName).HasMaxLength(50);
    builder.Property(e => e.StripeCustomerId).HasMaxLength(50);
    builder.Property(e => e.ClientImage).HasMaxLength(255);
    builder.Property(e => e.ClientName).HasMaxLength(50); 
    builder.Property(e => e.CreatedOn).HasColumnType("datetime");
    builder.Property(e => e.DefaultProductStationId).HasComment("The Default value will get from Product Station Table");
    builder.Property(e => e.ClientIdentifier).HasComment("This column is used for order prefix ");
    builder.Property(e => e.Email).HasMaxLength(50);
    builder.Property(e => e.LicenseNo).HasMaxLength(50);
    builder.Property(e => e.Mobile).HasMaxLength(50);
    builder.Property(e => e.Ongfid).HasColumnName("ONGFId");
    builder.Property(e => e.Phone).HasMaxLength(50); 
    builder.Property(e => e.Trnno)
        .HasMaxLength(20)
        .HasComment("also vat no")
        .HasColumnName("TRNNo");
    builder.Property(e => e.IsPaymentVerified).HasDefaultValueSql("((0))");
    builder.Property(e => e.Active).HasDefaultValueSql("((0))");
    builder.Property(e => e.UpdatedOn).HasColumnType("datetime"); 
  }
}
