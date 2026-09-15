using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.CommonAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class ScfolderLookupConfiguration : IEntityTypeConfiguration<ScfolderLookup>
{
  public void Configure(EntityTypeBuilder<ScfolderLookup> builder)
  {
    builder.ToTable("SCFolderLookup");

    // builder.Property(e => e.ScfolderGuid).HasConversion(scFolderGuid => scFolderGuid!.Value, value => new Guid(value));
    builder.Property(e => e.ScfolderLookupId).HasColumnName("SCFolderLookupId");
    builder.Property(e => e.ScfolderGuid).HasColumnName("SCFolderGuid");
    builder.Property(e => e.ScfolderName)
                  .HasMaxLength(150)
                  .HasColumnName("SCFolderName");
  }
}
