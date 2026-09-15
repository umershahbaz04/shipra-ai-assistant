using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class ProductStationConfiguration : IEntityTypeConfiguration<ProductStation>
{
  public void Configure(EntityTypeBuilder<ProductStation> builder)
  {
    builder.ToTable("ProductStation");
    #region conversion 
    builder.Property(e => e.ClientId).HasConversion(clientId => clientId!.Value, value => new ClientId(value!));
    builder.Property(e => e.UpdatedBy).HasConversion(id => id!.Value, value => new EmployeeId(value!));
    builder.Property(e => e.CreatedBy).HasConversion(id => id!.Value, value => new EmployeeId(value!));
    #endregion
    builder.Property(e => e.CreatedOn).HasColumnType("datetime");
    builder.Property(e => e.Name).HasMaxLength(50);
    builder.Property(e => e.StationCode).HasMaxLength(50);
    builder.Property(e => e.UpdatedOn).HasColumnType("datetime");
  }
}
