using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.CountryAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class CivilEntityExtendedConfiguration : IEntityTypeConfiguration<CivilEntityExtended>
{
  public void Configure(EntityTypeBuilder<CivilEntityExtended> builder)
  {
    builder.ToTable("CivilEntityExtended"); 
  }
}
