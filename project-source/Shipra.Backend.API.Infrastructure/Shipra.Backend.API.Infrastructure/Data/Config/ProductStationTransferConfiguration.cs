using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class ProductStationTransferConfiguration : IEntityTypeConfiguration<ProductStationTransfer>
{
  public void Configure(EntityTypeBuilder<ProductStationTransfer> builder)
  {

    builder.HasKey(e => e.ProductStaionTransferId);
    builder.ToTable("ProductStaionTransfer");

    #region conversion 
    builder.Property(e => e.ProductStaionTransferId).HasConversion(uuid => uuid!.Value, value => new ProductStaionTransferId(value));
    builder.Property(e => e.CreatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    builder.Property(e => e.UpdatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    #endregion

    builder.Property(e => e.ProductStaionTransferId).ValueGeneratedNever();
    builder.Property(e => e.CreatedOn).HasColumnType("datetime");
    builder.Property(e => e.ExpectedArrivalTime).HasColumnType("datetime");
    builder.Property(e => e.TrackingNo).HasMaxLength(20);
    builder.Property(e => e.TransferNo).HasMaxLength(20);
    builder.Property(e => e.UpdatedOn).HasColumnType("datetime");
  }
}
