using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.CommonAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class PaymentStatusLookupConfiguration : IEntityTypeConfiguration<PaymentStatusLookup>
{
  public void Configure(EntityTypeBuilder<PaymentStatusLookup> builder)
  {
    builder.HasKey(e => e.PaymentStatusId);
    builder.ToTable("PaymentStatusLookup");
    builder.Property(e => e.StatusName).HasMaxLength(100);
  }
}
