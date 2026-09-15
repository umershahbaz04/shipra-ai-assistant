using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class EmployeeTypeConfiguration : IEntityTypeConfiguration<EmployeeType>
{
  public void Configure(EntityTypeBuilder<EmployeeType> builder)
  {
    builder.HasKey(e => e.EmployeeTypeId);
    builder.ToTable("EmployeeType");

    builder.Property(e => e.EmployeeTypeName).HasMaxLength(150);
  }
}
