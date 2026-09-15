using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class OrderArchiveConfiguration : IEntityTypeConfiguration<OrderArchive>
{
  public void Configure(EntityTypeBuilder<OrderArchive> builder)
  {
    builder.ToTable("OrderArchive");
    builder.Property(e => e.OrderId).HasConversion(orderId => orderId!.Value, value => new OrderId(value!));
    builder.Property(e => e.CreatedBy).HasConversion(employeeId => employeeId!.Value, value => new EmployeeId(value!));
    builder.Property(e => e.ClientId).HasConversion(employeeId => employeeId!.Value, value => new ClientId(value!));

    builder.HasKey(x => x.OrderArchiveId);
    builder.Property(x => x.OrderArchiveId)
           .ValueGeneratedOnAdd();

    builder.Property(x => x.OrderJson)
           .IsRequired()
           .HasColumnType("nvarchar(max)");
  }
}
