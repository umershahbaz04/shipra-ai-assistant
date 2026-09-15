using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.StoresAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
internal class StoreUploadSampleFileConfiguration : IEntityTypeConfiguration<StoreUploadSampleFile>
{
  public void Configure(EntityTypeBuilder<StoreUploadSampleFile> builder)
  {
    builder.ToTable("StoreUploadSampleFile");
    builder.HasKey(x => x.StoreUploadSampleFileId);
    builder.Property(e => e.FilePath).HasColumnType("text");
  }
}

