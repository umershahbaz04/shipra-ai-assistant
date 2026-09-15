using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class ClientWebhookEventConfiguration : IEntityTypeConfiguration<ClientWebhookEvent>
{
  public void Configure(EntityTypeBuilder<ClientWebhookEvent> builder)
  {
    builder.ToTable("ClientWebhookEvent");
    #region conversion

    builder.Property(e => e.ClientId).HasConversion(clientId => clientId!.Value, value => new ClientId(value!));
    builder.Property(e => e.CreatedBy).HasConversion(clientId => clientId!.Value, value => new EmployeeId(value!));
    builder.Property(e => e.UpdatedBy).HasConversion(clientId => clientId!.Value, value => new EmployeeId(value!));
    #endregion
  }
}
