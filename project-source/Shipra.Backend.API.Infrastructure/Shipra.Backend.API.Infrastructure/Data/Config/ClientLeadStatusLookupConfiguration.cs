using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.LeadAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class ClientLeadStatusLookupConfiguration : IEntityTypeConfiguration<ClientLeadStatusLookup>
{
    public void Configure(EntityTypeBuilder<ClientLeadStatusLookup> builder)
    {
        builder.ToTable("ClientLeadStatusLookup");

        // Explicit PK — EF convention looks for 'ClientLeadStatusLookupId'
        // but the property is 'ClientLeadStatusId', so must be explicit
        builder.HasKey(e => e.ClientLeadStatusId);

        #region conversion
        builder.Property(e => e.ClientId).HasConversion(guid => guid!.Value, value => new ClientId(value!));
        builder.Property(e => e.CreatedBy).HasConversion(guid => guid!.Value, value => new EmployeeId(value!));
        builder.Property(e => e.UpdatedBy).HasConversion(guid => guid!.Value, value => new EmployeeId(value!));
        #endregion

        builder.Property(e => e.ClientLeadStatusId).ValueGeneratedOnAdd();
        builder.Property(e => e.Description).HasMaxLength(200);
        builder.Property(e => e.CreatedOn).HasColumnType("datetime");
        builder.Property(e => e.UpdatedOn).HasColumnType("datetime");
        builder.Property(e => e.Active).HasDefaultValueSql("((1))");
    }
}
