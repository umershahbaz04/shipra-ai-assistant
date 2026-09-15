using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.MenuAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class MenuOtherRouteConfiguration : IEntityTypeConfiguration<MenuOtherRoute>
{
  public void Configure(EntityTypeBuilder<MenuOtherRoute> builder)
  {
    builder.ToTable("MenuOtherRoute");

    builder.Property(e => e.RoutePath).HasMaxLength(250);
    builder.Property(e => e.TabBarTitle).HasMaxLength(50);
  }
}
