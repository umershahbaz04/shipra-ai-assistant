using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class SaleChannelConfiguration : IEntityTypeConfiguration<Core.SaleChannelConfigAggregate.SaleChannelConfig>
{
  public void Configure(EntityTypeBuilder<Core.SaleChannelConfigAggregate.SaleChannelConfig> builder)
  {
    builder.HasKey(e => e.SaleChannelConfigId);
    builder.ToTable("SaleChannelConfig");

    #region conversion

    builder.Property(e => e.ClientId).HasConversion(orderId => orderId!.Value, value => new ClientId(value!));
    builder.Property(e => e.CreatedBy).HasConversion(employeeId => employeeId!.Value, value => new EmployeeId(value!));
    builder.Property(e => e.UpdatedBy).HasConversion(employeeId => employeeId!.Value, value => new EmployeeId(value!));
    #endregion
    builder.HasIndex(e => e.SaleChannelKey, "UQ__SaleChan__73B13E396B17F208").IsUnique();
    builder.Property(e => e.Config).HasColumnType("text");
    builder.Property(e => e.CreatedOn).HasColumnType("datetime");
    builder.Property(e => e.SaleChannelKey).HasMaxLength(250);
    builder.Property(e => e.UpdateOn).HasColumnType("datetime");
  }
}
