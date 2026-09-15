using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.CatalougeAggregate;
using Shipra.Backend.API.Core.CommonAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class CatalogueConfiguration : IEntityTypeConfiguration<Catalogue>
{
  public void Configure(EntityTypeBuilder<Catalogue> builder)
  {
    builder.ToTable("Catalogue"); 
    builder.Property(e => e.ClientId).HasMaxLength(36);
  }
}
