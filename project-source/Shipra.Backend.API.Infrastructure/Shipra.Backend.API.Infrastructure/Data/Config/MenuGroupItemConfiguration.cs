using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.MenuAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class MenuGroupItemConfiguration : IEntityTypeConfiguration<MenuGroupItem>
{
  public void Configure(EntityTypeBuilder<MenuGroupItem> builder)
  {
    builder.HasKey(e => e.MenuGroupItemId);
    builder.ToTable("MenuGroupItem");
  }
}
