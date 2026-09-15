using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class DriverConfiguration : IEntityTypeConfiguration<Driver>
{
  public void Configure(EntityTypeBuilder<Driver> builder)
  {
    builder.HasKey(e => e.DriverId);
    builder.ToTable("Driver");

    #region conversions 
    builder.Property(e => e.DriverId).HasConversion(uuid => uuid!.Value, value => new DriverId(value));
    builder.Property(e => e.ClientId).HasConversion(uuid => uuid!.Value, value => new ClientId(value));
    builder.Property(e => e.EmployeeId).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    builder.Property(e => e.CreatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    builder.Property(e => e.UpdatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    #endregion

    builder.HasIndex(e => e.AppUsername, "UK_Drivers_Username").IsUnique();

    builder.Property(e => e.DriverId).ValueGeneratedNever();
    builder.Property(e => e.Active).HasDefaultValueSql("((1))");
    builder.Property(e => e.AppPassword).HasMaxLength(20);
    builder.Property(e => e.AppUsername).HasMaxLength(20);
    builder.Property(e => e.CreatedOn).HasColumnType("datetime");
    builder.Property(e => e.DriverCode).HasMaxLength(20);
    builder.Property(e => e.UpdatedOn).HasColumnType("datetime");
  }
}
