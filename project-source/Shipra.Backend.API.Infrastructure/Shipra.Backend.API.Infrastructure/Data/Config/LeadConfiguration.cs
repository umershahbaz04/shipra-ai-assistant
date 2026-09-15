using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.LeadAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class LeadConfiguration : IEntityTypeConfiguration<Lead>
{
    public void Configure(EntityTypeBuilder<Lead> builder)
    {
        builder.HasKey(x => x.LeadId);
        builder.ToTable("Leads");

        #region conversion
        builder.Property(e => e.LeadId).HasConversion(leadId => leadId!.Value, value => new LeadId(value!));
        builder.Property(e => e.ClientId).HasConversion(clientId => clientId!.Value, value => new ClientId(value!));
        builder.Property(e => e.SalespersonId).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
        builder.Property(e => e.CreatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
        builder.Property(e => e.UpdatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
        #endregion

        builder.Property(e => e.LeadId).ValueGeneratedNever();
        builder.Property(e => e.PhoneNumber).HasMaxLength(50);
        builder.Property(e => e.ProductName).HasMaxLength(255);

    }
}
