using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.ReturnAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class ReturnTrackingHistoryConfiguration : IEntityTypeConfiguration<ReturnTrackingHistory>
{
  public void Configure(EntityTypeBuilder<ReturnTrackingHistory> builder)
  {
    builder.ToTable("ReturnTrackingHistory");
    #region convrsion
    builder.Property(e => e.ReturnTrackingHistoryId).HasConversion(uuid => uuid!.Value, value => new ReturnTrackingHistoryId(value)); 
    builder.Property(e => e.ReturnId).HasConversion(uuid => uuid!.Value, value => new ReturnId(value));
    builder.Property(e => e.CreatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    #endregion

    builder.Property(e => e.ReturnTrackingHistoryId).ValueGeneratedNever();
    builder.Property(e => e.CreatedByName).HasMaxLength(350);
    builder.Property(e => e.CreatedOn).HasColumnType("datetime");  
  }
}
