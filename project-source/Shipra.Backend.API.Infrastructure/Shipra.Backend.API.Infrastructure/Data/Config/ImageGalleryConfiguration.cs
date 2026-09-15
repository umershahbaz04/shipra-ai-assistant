using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.ProductAggregate;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class ImageGalleryConfiguration : IEntityTypeConfiguration<ImageGallery>
{
  public void Configure(EntityTypeBuilder<ImageGallery> builder)
  {
    builder.ToTable("ImageGallery");
    #region convrsion
    builder.Property(e => e.ClientId).HasConversion(uuid => uuid!.Value, value => new ClientId(value)); 
    builder.Property(e => e.CreatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value)); 
    #endregion
    builder.HasKey(x => x.ImageGalleryId); 
    builder.Property(x => x.ClientId).IsRequired();

    builder.Property(x => x.ImageUrl)
        .HasMaxLength(500)
        .IsRequired(); 
    builder.Property(x => x.Description).HasMaxLength(255); 
    builder.Property(x => x.CreatedOn); 
    builder.Property(x => x.Active);
  }
}
