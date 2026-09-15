using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class OrderUplaodSampleFileConfiguration : IEntityTypeConfiguration<OrderUplaodSampleFile>
{
  public void Configure(EntityTypeBuilder<OrderUplaodSampleFile> builder)
  {
    builder.ToTable("OrderUplaodSampleFile");

    builder.Property(e => e.FilePath).HasColumnType("text");
  }
}
