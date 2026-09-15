using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ActivityLogAggregate;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLog>
{
  public void Configure(EntityTypeBuilder<ActivityLog> builder)
  {
    builder.ToTable("ActivityLog");

    #region conversion
    builder.Property(e => e.ActivityLogId).HasConversion(uuid => uuid!.Value, value => new ActivityLogId(value));
    builder.Property(e => e.ClientId).HasConversion(uuid => uuid!.Value, value => new ClientId(value));
    builder.Property(e => e.EmployeeId).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    #endregion

    builder.Property(e => e.ActivityLogId).ValueGeneratedNever();
    builder.Property(e => e.Aggregate).HasMaxLength(150);
    builder.Property(e => e.CreateOn).HasColumnType("datetime");
    builder.Property(e => e.Request).HasColumnType("text");
    builder.Property(e => e.Response).HasColumnType("text");
  }
}
