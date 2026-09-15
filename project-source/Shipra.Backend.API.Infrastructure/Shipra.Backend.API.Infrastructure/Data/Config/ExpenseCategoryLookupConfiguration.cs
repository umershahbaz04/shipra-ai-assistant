using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.ProductAggregate;
using Shipra.Backend.API.Core.CommonAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class ExpenseCategoryLookupConfiguration : IEntityTypeConfiguration<ExpenseCategoryLookup>
{
  public void Configure(EntityTypeBuilder<ExpenseCategoryLookup> builder)
  {
    builder.HasKey(e => e.ExpenseCategoryId);
    builder.ToTable("ExpenseCategoryLookup");
    builder.Property(e => e.ExpenseCategoryName).HasMaxLength(50);
  }
}
