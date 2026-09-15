using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class EmployeeAddressConfiguration : IEntityTypeConfiguration<EmployeeAddress>
{
  public void Configure(EntityTypeBuilder<EmployeeAddress> builder)
  {

    builder.ToTable("EmployeeAddress");

    #region conversion
    builder.Property(e => e.EmployeeId).HasConversion(employeeId => employeeId!.Value, value => new EmployeeId(value!)); 
    #endregion 
    builder.Property(e => e.BuildingName).HasMaxLength(50);
    builder.Property(e => e.FullAddress).HasMaxLength(200);
    builder.Property(e => e.HouseNo).HasMaxLength(20);
    builder.Property(e => e.Landmark).HasMaxLength(50);
    builder.Property(e => e.Latitude).HasColumnType("decimal(8, 6)");
    builder.Property(e => e.Longitude).HasColumnType("decimal(9, 6)");
    builder.Property(e => e.StreetAddress).HasMaxLength(150);
    builder.Property(e => e.StreetAddress2).HasMaxLength(500);
    builder.Property(e => e.Zip).HasMaxLength(10);

  }

}
