using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.CountryAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class StateConfiguration : IEntityTypeConfiguration<State>
{
  public void Configure(EntityTypeBuilder<State> builder)
  {
    builder.ToTable("State");

    builder.Property(e => e.Name).HasMaxLength(50);
  }
}
