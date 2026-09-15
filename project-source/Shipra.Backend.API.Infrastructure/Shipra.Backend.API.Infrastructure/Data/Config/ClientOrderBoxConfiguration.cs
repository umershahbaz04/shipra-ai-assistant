using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class ClientOrderBoxConfiguration : IEntityTypeConfiguration<ClientOrderBox>
{
  public void Configure(EntityTypeBuilder<ClientOrderBox> builder)
  {
    builder.ToTable("ClientOrderBox");


    #region conversion
    builder.Property(e => e.ClientId).HasConversion(orderId => orderId!.Value, value => new ClientId(value!));
    builder.Property(e => e.CreatedBy).HasConversion(orderId => orderId!.Value, value => new  EmployeeId(value!));
    builder.Property(e => e.UpdatedBy).HasConversion(orderId => orderId!.Value, value => new  EmployeeId(value!));
    #endregion

    builder.Property(e => e.BoxName).HasMaxLength(50);
    builder.Property(e => e.Height).HasColumnType("decimal(18, 2)");
    builder.Property(e => e.Length).HasColumnType("decimal(18, 2)");
    builder.Property(e => e.Volume).HasColumnType("decimal(18, 2)");
    builder.Property(e => e.Width).HasColumnType("decimal(18, 2)");
  }
}
