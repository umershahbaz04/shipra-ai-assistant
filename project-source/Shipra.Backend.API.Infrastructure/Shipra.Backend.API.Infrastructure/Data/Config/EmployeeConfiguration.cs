using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
  public void Configure(EntityTypeBuilder<Employee> builder)
  {

    builder.HasKey(e => e.EmployeeId);
    builder.ToTable("Employee");

    #region conversion
    builder.Property(e => e.EmployeeId).HasConversion(employeeId => employeeId!.Value, value => new EmployeeId(value!));
    builder.Property(e => e.ClientId).HasConversion(clientId => clientId!.Value, value => new ClientId(value!));
    builder.Property(e => e.UpdatedBy).HasConversion(id => id!.Value, value => new EmployeeId(value!));
    builder.Property(e => e.CreatedBy).HasConversion(id => id!.Value, value => new EmployeeId(value!));
    #endregion


    builder.HasIndex(e => e.EmployeeCode, "UQ_EMPCode").IsUnique();

    builder.Property(e => e.EmployeeId).ValueGeneratedNever(); 
    builder.Property(e => e.CreatedOn).HasColumnType("datetime");
    builder.Property(e => e.DateOfBirth).HasColumnType("datetime");
    builder.Property(e => e.EmployeeCode).HasMaxLength(50);
    builder.Property(e => e.EmployeeImage).HasMaxLength(255);
    builder.Property(e => e.EmployeeName).HasMaxLength(50); 
    builder.Property(e => e.MobileNo).HasMaxLength(50);
    builder.Property(e => e.PhoneNo).HasMaxLength(50);
    builder.Property(e => e.UpdatedOn).HasColumnType("datetime");
    builder.Property(e => e.WorkEmail).HasMaxLength(100); 
  }
}
