using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class EmployeeTypeLookupConfiguration : IEntityTypeConfiguration<EmployeeTypeLookup>
{
  public void Configure(EntityTypeBuilder<EmployeeTypeLookup> builder)
  {
    builder.HasKey(e => e.EmployeeTypeId);
    builder.ToTable("EmployeeTypeLookup");
    builder.Property(e => e.EmployeeTypeName).HasMaxLength(50);
  }
}
