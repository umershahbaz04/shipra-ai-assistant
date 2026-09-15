using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.DocumentAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;

public class DocumentTemplateConfigConfiguration : IEntityTypeConfiguration<DocumentTemplateConfig>
{
  public void Configure(EntityTypeBuilder<DocumentTemplateConfig> builder)
  {
    builder.ToTable("DocumentTemplateConfig");
    #region conversions 
    builder.Property(e => e.ClientId).HasConversion(uuid => uuid!.Value, value => new ClientId(value)); 
    builder.Property(e => e.DocumentTemplateConfigId).HasConversion(uuid => uuid!.Value, value => new DocumentTemplateConfigId(value));
    builder.Property(e => e.CreatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    builder.Property(e => e.UpdatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    #endregion

    builder.Property(e => e.DocumentTemplateConfigId).ValueGeneratedNever();
    builder.Property(e => e.CreatedOn).HasColumnType("datetime");
    builder.Property(e => e.TemplateName).HasMaxLength(50);
    builder.Property(e => e.UpdatedOn).HasColumnType("datetime");
  }
}
