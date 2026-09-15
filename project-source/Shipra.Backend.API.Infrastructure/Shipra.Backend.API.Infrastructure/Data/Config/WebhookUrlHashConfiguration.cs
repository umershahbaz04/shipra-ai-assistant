using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
internal class WebhookUrlHashConfiguration : IEntityTypeConfiguration<WebhookUrlHash>
{
  public void Configure(EntityTypeBuilder<WebhookUrlHash> builder)
  {
    builder.ToTable("WebhookUrlHash");

    builder.Property(e => e.WebhookUrlHashId).ValueGeneratedNever();
    builder.Property(e => e.CreatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    builder.Property(e => e.CreatedOn).HasColumnType("datetime");
  }
}
