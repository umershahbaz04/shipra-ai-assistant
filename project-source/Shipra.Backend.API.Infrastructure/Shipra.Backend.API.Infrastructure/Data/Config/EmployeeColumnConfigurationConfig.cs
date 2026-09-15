using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class EmployeeColumnConfigurationConfig : IEntityTypeConfiguration<EmployeeColumnConfiguration>
{
  public void Configure(EntityTypeBuilder<EmployeeColumnConfiguration> builder)
  {
    builder.ToTable("EmployeeColumnConfiguration");
    #region convrsion
    builder.Property(e => e.ClientId).HasConversion(uuid => uuid!.Value, value => new ClientId(value));
    builder.Property(e => e.EmployeeId).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value)); 
    #endregion
    builder.HasKey(x => x.ColumnConfigurationId); 
    builder.Property(x => x.ColumnConfigurationId).ValueGeneratedOnAdd(); 
    builder.Property(x => x.EmployeeId).IsRequired(); 
    builder.Property(x => x.TableName).HasMaxLength(100).IsRequired(); 
    builder.Property(x => x.Config).HasMaxLength(500).IsRequired(); 
  }
}
