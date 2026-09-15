using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.ContributorAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class McconfigConfiguration : IEntityTypeConfiguration<Mcconfig>
{
  public void Configure(EntityTypeBuilder<Mcconfig> builder)
  {
    builder.HasKey(e => e.Mcid);

    builder.ToTable("MCConfig");
    builder.Property(e => e.Mcid).HasColumnName("MCId");
    builder.Property(e => e.Description).HasMaxLength(250);
    builder.Property(e => e.Parameter).HasMaxLength(50);
    builder.Property(e => e.Value).HasMaxLength(250);
  }
}
