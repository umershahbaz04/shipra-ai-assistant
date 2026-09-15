using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.StoresAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class StoreConfiguration : IEntityTypeConfiguration<Store>
{
  public void Configure(EntityTypeBuilder<Store> builder)
  {
    builder.HasKey(e => e.StoreId);

    #region conversion 
    builder.Property(e => e.ClientId).HasConversion(uuid => uuid!.Value, value => new ClientId(value));
    builder.Property(e => e.CreatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    builder.Property(e => e.UpdatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    #endregion
    builder.Property(e => e.Active).HasDefaultValueSql("((1))");
    builder.Property(e => e.CreatedOn).HasColumnType("datetime");
    builder.Property(e => e.CustomerServiceNo).HasMaxLength(50);
    builder.Property(e => e.Email).HasMaxLength(50);
    builder.Property(e => e.LicenseNo).HasMaxLength(50); 
    builder.Property(e => e.Phone).HasMaxLength(50); 
    builder.Property(e => e.StoreCompany).HasMaxLength(50);
    builder.Property(e => e.StoreImage).HasMaxLength(255);
    builder.Property(e => e.StoreName).HasMaxLength(200);
    builder.Property(e => e.UpdatedOn).HasColumnType("datetime");
    builder.Property(e => e.Urls)
        .HasMaxLength(500)
        .HasColumnName("URLs");
  }
}

