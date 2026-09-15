using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.AccountAggregate;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class CarrierPaymentConfiguration : IEntityTypeConfiguration<CarrierPaymentSettlement>
{
  public void Configure(EntityTypeBuilder<CarrierPaymentSettlement> builder)
  {
    builder.ToTable("CarrierPaymentSettlement");

    #region conversions 
    builder.Property(e => e.CarrierPaymentSettlementId).HasConversion(uuid => uuid!.Value, value => new CarrierPaymentSettlementId(value)); 
    builder.Property(e => e.CreatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    builder.Property(e => e.UpdatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    #endregion

    builder.Property(e => e.CarrierPaymentSettlementId).ValueGeneratedNever();
    builder.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
    builder.Property(e => e.AmountReceived).HasColumnType("decimal(18, 2)");
    builder.Property(e => e.CreatedOn).HasColumnType("datetime");
    builder.Property(e => e.PaymentDate).HasColumnType("datetime");
    builder.Property(e => e.PaymentRef).HasMaxLength(50);
    builder.Property(e => e.UpdatedOn).HasColumnType("datetime");
  }
}
