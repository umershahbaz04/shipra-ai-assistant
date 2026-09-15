using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.LeadAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class LeadStatusLookupConfiguration : IEntityTypeConfiguration<LeadStatusLookup>
{
    public void Configure(EntityTypeBuilder<LeadStatusLookup> builder)
    {
        builder.HasKey(x => x.LeadStatusId);
        builder.ToTable("LeadStatusLookups");

        builder.Property(e => e.LeadStatusId).ValueGeneratedNever();
        builder.Property(e => e.Description).HasMaxLength(100);
        builder.Property(e => e.Active).HasDefaultValue(true);
    }
}
