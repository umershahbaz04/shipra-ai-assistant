using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ContractAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class UOMLookupConfiguration : IEntityTypeConfiguration<UOMLookup>
{
  public void Configure(EntityTypeBuilder<UOMLookup> builder)
  {
    builder.HasKey(e => e.UOMId);
    builder.ToTable("UOMLookup");
  }
}
