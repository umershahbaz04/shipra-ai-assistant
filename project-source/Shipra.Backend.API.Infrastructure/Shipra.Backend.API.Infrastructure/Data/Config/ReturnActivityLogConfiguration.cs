using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ReturnAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class ReturnActivityLogConfiguration : IEntityTypeConfiguration<ReturnActivityLog>
{
  public void Configure(EntityTypeBuilder<ReturnActivityLog> builder)
  {
    #region convrsion
    builder.Property(e => e.ReturnId).HasConversion(uuid => uuid!.Value, value => new ReturnId(value));
    builder.Property(e => e.ReturnActivtyLogId).HasConversion(uuid => uuid!.Value, value => new ReturnActivtyLogId(value));
    #endregion
    builder.HasKey(e => e.ReturnActivtyLogId);
    builder.ToTable("ReturnActivityLog");

    builder.Property(e => e.ReturnActivtyLogId).ValueGeneratedNever();
    builder.Property(e => e.ActivityLog).HasColumnType("text");
    builder.Property(e => e.CreatedOn).HasColumnType("datetime");
  }
}
