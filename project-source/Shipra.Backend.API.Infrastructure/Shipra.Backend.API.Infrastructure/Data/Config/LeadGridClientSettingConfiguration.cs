using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.LeadAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class LeadGridClientSettingConfiguration : IEntityTypeConfiguration<LeadGridClientSetting>
{
    public void Configure(EntityTypeBuilder<LeadGridClientSetting> builder)
    {
        builder.HasKey(e => e.LeadGridClientSettingId).HasName("PK_LeadGridClientSetting");

        builder.ToTable("LeadGridClientSetting");

        #region conversion
        builder.Property(e => e.LeadGridClientSettingId).HasConversion(guid => guid!.Value, value => new LeadGridClientSettingId(value!));
        builder.Property(e => e.ClientId).HasConversion(guid => guid!.Value, value => new ClientId(value!));
        builder.Property(e => e.CreatedBy).HasConversion(guid => guid!.Value, value => new EmployeeId(value!));
        builder.Property(e => e.UpdatedBy).HasConversion(guid => guid!.Value, value => new EmployeeId(value!));
        #endregion

        builder.Property(e => e.LeadGridClientSettingId).ValueGeneratedNever();
        builder.Property(e => e.Active).HasDefaultValueSql("((1))");
        builder.Property(e => e.CreatedOn).HasColumnType("datetime");
        builder.Property(e => e.DashboardStatusValue).HasMaxLength(250);
        builder.Property(e => e.UpdatedOn).HasColumnType("datetime");
    }
}
