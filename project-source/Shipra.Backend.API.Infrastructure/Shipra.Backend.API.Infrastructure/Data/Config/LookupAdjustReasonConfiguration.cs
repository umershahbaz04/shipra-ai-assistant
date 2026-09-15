using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.CommonAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class LookupAdjustReasonConfiguration : IEntityTypeConfiguration<LookupAdjustReason>
{
  public void Configure(EntityTypeBuilder<LookupAdjustReason> builder)
  {
    builder.HasKey(e => e.LookupAdjustReasonId);
    builder.ToTable("LookupAdjustReason");
    builder.Property(e => e.Reason).HasMaxLength(50);
  }
}

