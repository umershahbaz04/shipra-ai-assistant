using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.CarrierReturnReportAggregate;
using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class CarrierReturnReportConfiguration : IEntityTypeConfiguration<CarrierReturnReport>
{
  public void Configure(EntityTypeBuilder<CarrierReturnReport> builder)
  {
    builder.HasKey(e => e.CarrierRrid);

    builder.ToTable("CarrierReturnReport");
    #region conversion 
    builder.Property(e => e.CarrierRrid).HasConversion(uid => uid!.Value, value => new CarrierRRId(value!)); 
    builder.Property(e => e.CreatedBy).HasConversion(id => id!.Value, value => new EmployeeId(value!)); 
    #endregion

    builder.Property(e => e.CarrierRrid)
        .ValueGeneratedNever()
        .HasComment("CarrierReturnReportId")
        .HasColumnName("CarrierRRId");
    builder.Property(e => e.CreatedOn).HasColumnType("datetime");
    builder.Property(e => e.ReturnReportNo).HasMaxLength(20);
  }
}
