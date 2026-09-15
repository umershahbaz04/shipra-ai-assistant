using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class ClientCarrierTrackingStatusConfiguration : IEntityTypeConfiguration<ClientCarrierTrackingStatus>
{
  public void Configure(EntityTypeBuilder<ClientCarrierTrackingStatus> builder)
  {
    builder.HasKey(e => e.ClientCarrierTrackingStatusId);
    #region conversion 
    builder.Property(e => e.ClientId).HasConversion(clientId => clientId!.Value, value => new ClientId(value!));
    #endregion

    builder.ToTable("ClientCarrierTrackingStatus");

    builder.Property(e => e.TrackingStatus)
        .HasMaxLength(50)
        .IsUnicode(false);
    builder.Property(e => e.TrackingStatusAr).HasMaxLength(50);
  }
}
