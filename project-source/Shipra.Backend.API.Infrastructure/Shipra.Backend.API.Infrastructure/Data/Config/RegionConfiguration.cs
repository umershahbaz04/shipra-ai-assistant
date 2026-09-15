using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.ProjectAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
//public class RegionConfiguration : IEntityTypeConfiguration<Region>
//{
//  public void Configure(EntityTypeBuilder<Region> builder)
//  {
//    builder.ToTable("Region");

//    builder.Property(e => e.Name).HasMaxLength(150);
//  }
//}
