using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.CommonAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class AddressTypeLookupConfiguration : IEntityTypeConfiguration<AddressTypeLookup>
{
  public void Configure(EntityTypeBuilder<AddressTypeLookup> builder)
  {
    builder.HasKey(e => e.AddressTypeId);
    builder.ToTable("AddressTypeLookup");
    builder.Property(e => e.AddressTypeName).HasMaxLength(100);
  }
}
