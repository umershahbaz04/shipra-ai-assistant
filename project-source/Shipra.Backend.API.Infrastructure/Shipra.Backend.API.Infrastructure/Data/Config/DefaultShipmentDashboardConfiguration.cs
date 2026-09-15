using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.SettingOperationDashboardAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class DefaultShipmentDashboardConfiguration : IEntityTypeConfiguration<DefaultShipmentDashboard>
{
  public void Configure(EntityTypeBuilder<DefaultShipmentDashboard> entity)
  {
    entity.HasKey(e => e.DefaultShipmentDashboardId).HasName("PK_SettingOperationDashboard");

    entity.ToTable("DefaultShipmentDashboard");

    entity.Property(e => e.DashboardStatusId).HasColumnName("DashboardStatusID");
    entity.Property(e => e.DashboardStatusName).HasMaxLength(50);
    entity.Property(e => e.DashboardStatusValue).HasMaxLength(150);
  }
}
