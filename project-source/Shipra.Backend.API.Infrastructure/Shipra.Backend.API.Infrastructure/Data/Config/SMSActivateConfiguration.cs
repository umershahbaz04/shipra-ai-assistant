using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.PaymentProcessAggregate;
using Shipra.Backend.API.Core.SMSProcessAggregate;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class SMSActivateConfiguration : IEntityTypeConfiguration<SMSActivate>
{
  public void Configure(EntityTypeBuilder<SMSActivate> builder)
  {
    builder.ToTable("SMSActivate");

    #region conversion 
    builder.Property(e => e.ClientId).HasConversion(guid => guid!.Value, value => new ClientId(value!));
    builder.Property(e => e.CreatedBy).HasConversion(guid => guid!.Value, value => new EmployeeId(value!));
    builder.Property(e => e.UpdatedBy).HasConversion(guid => guid!.Value, value => new EmployeeId(value!));
    #endregion


    builder.ToTable("SMSActivate");



    builder.Property(e => e.SMSActivateId) 
        .HasColumnName("SMSActivateId");
    builder.Property(e => e.Active).HasDefaultValueSql("((1))");
    builder.Property(e => e.Config).HasColumnType("text");
    builder.Property(e => e.CreatedOn).HasColumnType("datetime");
    builder.Property(e => e.SMSLookupId).HasColumnName("SMSLookupId");
    builder.Property(e => e.UpdateOn).HasColumnType("datetime");
  }
}
