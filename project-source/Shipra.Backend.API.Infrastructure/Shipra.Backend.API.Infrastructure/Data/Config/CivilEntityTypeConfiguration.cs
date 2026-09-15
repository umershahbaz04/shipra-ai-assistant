using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.CountryAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class CivilEntityTypeConfiguration : IEntityTypeConfiguration<CivilEntityType>
{
  public void Configure(EntityTypeBuilder<CivilEntityType> builder)
  {
    builder.ToTable("CivilEntityType");
    builder.HasKey(e => e.CivilEntityTypeId);
    builder.Property(e => e.TypeName).HasMaxLength(100);
  }
}
