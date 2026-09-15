using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.CatalougeAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class ShopifySessionConfiguration : IEntityTypeConfiguration<ShopifySession>
{
  public void Configure(EntityTypeBuilder<ShopifySession> builder)
  {
    builder.ToTable("ShopifySession");

    builder.Property(e => e.Id).HasMaxLength(250); 
  }
}
