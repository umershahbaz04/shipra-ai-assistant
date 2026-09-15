using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class InventoryTransactionConfiguration : IEntityTypeConfiguration<InventoryTransaction>
{
  public void Configure(EntityTypeBuilder<InventoryTransaction> builder)
  {
      builder.HasKey(e => e.InventoryTransactionId);
      builder.ToTable("InventoryTransaction");

      #region conversion
      builder.Property(e => e.CreatedBy).HasConversion(createdBy => createdBy!.Value, value => new EmployeeId(value!));
      #endregion

      builder.Property(e => e.ProductVariantId).IsRequired();
      builder.Property(e => e.ProductStationId).IsRequired();
      builder.Property(e => e.TransactionTypeId).HasColumnName("InventoryTransactionTypeId").IsRequired();
      builder.Property(e => e.Quantity).HasColumnType("decimal(18,4)").IsRequired();
      builder.Property(e => e.PreviousQuantity).HasColumnType("decimal(18,4)");
      builder.Property(e => e.NewQuantity).HasColumnType("decimal(18,4)");
      builder.Property(e => e.ReferenceId).HasMaxLength(100);
      builder.Property(e => e.Comment).HasMaxLength(500);

      builder.Property(e => e.CreatedOn).HasColumnType("datetime").IsRequired();
  }
}
