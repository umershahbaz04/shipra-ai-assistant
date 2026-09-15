using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.StoresAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class SaleChannelVariantConfiguration : IEntityTypeConfiguration<SaleChannelVariant>
{
  public void Configure(EntityTypeBuilder<SaleChannelVariant> builder)
  {
      builder.HasKey(e => e.SaleChannelVariantId);
      builder.ToTable("SaleChannelVariant");

      #region conversion
      builder.Property(e => e.CreatedBy).HasConversion(createdBy => createdBy!.Value, value => new EmployeeId(value!));
      builder.Property(e => e.UpdatedBy).HasConversion(updatedBy => updatedBy!.Value, value => new EmployeeId(value!));
      #endregion

      builder.Property(e => e.ProductVariantId).IsRequired();
      builder.Property(e => e.ExternalVariantId).HasMaxLength(100);
      builder.Property(e => e.ExternalSKU).HasMaxLength(100);
      builder.Property(e => e.ExternalBarcode).HasMaxLength(100);
      builder.Property(e => e.ExternalCatalogId).HasMaxLength(100);
      builder.Property(e => e.ExternalCatalogIdType).HasMaxLength(50);
       
  }
}
