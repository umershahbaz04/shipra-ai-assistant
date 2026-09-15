using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class OrderPODFilesConfiguration : IEntityTypeConfiguration<OrderPODFile>
{
  public void Configure(EntityTypeBuilder<OrderPODFile> builder)
  {
    builder.ToTable("OrderPODFiles");

    builder.Property(e => e.OrderPodfileId)
        .ValueGeneratedNever()
        .HasColumnName("OrderPODFileId");

    #region conversion
    builder.Property(e => e.OrderPodfileId).HasConversion(uuid => uuid!.Value, value => new OrderPODFileId(value));
    builder.Property(e => e.OrderId).HasConversion(uuid => uuid!.Value, value => new OrderId(value));
    builder.Property(e => e.CreatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    builder.Property(e => e.UpdatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    #endregion

    builder.Property(e => e.Comment).HasMaxLength(250);
    builder.Property(e => e.CreatedOn).HasColumnType("datetime");
    builder.Property(e => e.FilePath).HasMaxLength(250);
    builder.Property(e => e.UpdatedOn).HasColumnType("datetime");

  }
}
