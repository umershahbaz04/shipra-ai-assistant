using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class ClientTaxConfiguration : IEntityTypeConfiguration<ClientTax>
{
  public void Configure(EntityTypeBuilder<ClientTax> builder)
  {
    builder.ToTable("ClientTax");
    #region conversions 
    builder.Property(e => e.ClientId).HasConversion(uuid => uuid!.Value, value => new ClientId(value));
    builder.Property(e => e.CreatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    builder.Property(e => e.UpdatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    #endregion

    builder.Property(e => e.Active).HasDefaultValueSql("((1))");
    builder.Property(e => e.CreatedOn).HasColumnType("datetime");
    builder.Property(e => e.UpdateOn).HasColumnType("datetime");
  }
}
