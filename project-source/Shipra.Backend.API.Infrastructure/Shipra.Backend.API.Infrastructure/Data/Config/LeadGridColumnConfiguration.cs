using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.LeadAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class LeadGridColumnConfiguration : IEntityTypeConfiguration<LeadGridColumn>
{
    public void Configure(EntityTypeBuilder<LeadGridColumn> builder)
    {
        builder.ToTable("LeadGridColumn");

        #region conversion
        builder.Property(e => e.ClientId).HasConversion(guid => guid!.Value, value => new ClientId(value!));
        builder.Property(e => e.CreatedBy).HasConversion(guid => guid!.Value, value => new EmployeeId(value!));
        builder.Property(e => e.UpdatedBy).HasConversion(guid => guid!.Value, value => new EmployeeId(value!));
        #endregion

        builder.Property(e => e.ColumnName).HasMaxLength(100);
        builder.Property(e => e.CreatedOn).HasColumnType("datetime");
        builder.Property(e => e.UpdatedOn).HasColumnType("datetime");
    }
}
