using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class GenderLookupConfiguration : IEntityTypeConfiguration<GenderLookup>
{
  public void Configure(EntityTypeBuilder<GenderLookup> builder)
  {
    builder.HasKey(e => e.GenderId);
    builder.ToTable("GenderLookup");
    builder.Property(e => e.GenderName).HasMaxLength(50);
  }
}
