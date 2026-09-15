using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.AccountAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class CpsettlementPodFileConfiguration : IEntityTypeConfiguration<CPSettlementPopFile>
{
  public void Configure(EntityTypeBuilder<CPSettlementPopFile> builder)
  { 
    #region conversion 
    builder.Property(e => e.CarrierPaymentSettlementId).HasConversion(uid => uid!.Value, value => new CarrierPaymentSettlementId(value!));
    builder.Property(e => e.CreatedBy).HasConversion(id => id!.Value, value => new EmployeeId(value!));
    builder.Property(e => e.UpdatedBy).HasConversion(id => id!.Value, value => new EmployeeId(value!));
    #endregion

    builder.HasKey(e => e.CpsettlementPopFileId).HasName("PK_CPSettlementPodFile");

    builder.ToTable("CPSettlementPopFile");

    builder.Property(e => e.CpsettlementPopFileId)
        .HasComment("CPSettlementPopFileI is a short form form table CarrierPaymentSettlementPopFile")
        .HasColumnName("CPSettlementPopFileId");
    builder.Property(e => e.Active).HasDefaultValueSql("((1))");
    builder.Property(e => e.CreatedOn).HasColumnType("datetime");
    builder.Property(e => e.Extension).HasMaxLength(10);
    builder.Property(e => e.FilePath).HasColumnType("text");
    builder.Property(e => e.UpdateOn).HasColumnType("datetime");
  }
}
