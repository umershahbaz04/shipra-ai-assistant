using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class OrderDraftConfiguration : IEntityTypeConfiguration<OrderDraft>
{
  public void Configure(EntityTypeBuilder<OrderDraft> builder)
  {
    builder.ToTable("OrderDraft");

    builder.HasKey(o => o.OrderDraftId);
    #region conversion 
    builder.Property(e => e.CreatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    builder.Property(e => e.UpdatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value)); 
    builder.Property(e => e.ClientId).HasConversion(uuid => uuid!.Value, value => new ClientId(value)); 

    #endregion

    builder.Property(o => o.OrderNo)
           .IsRequired()
           .HasMaxLength(50);

    builder.Property(o => o.OrderInfo)
           .IsRequired()
           .HasColumnType("nvarchar(max)"); 
  }
}
