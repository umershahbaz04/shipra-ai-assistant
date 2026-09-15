using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.ExpenseAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class ExpenseCategoryConfiguration : IEntityTypeConfiguration<ExpenseCategory>
{
  public void Configure(EntityTypeBuilder<ExpenseCategory> builder)
  {

    #region conversion
    builder.Property(e => e.ClientId).HasConversion(clientId => clientId!.Value, value => new ClientId(value!));
    builder.Property(e => e.UpdatedBy).HasConversion(id => id!.Value, value => new EmployeeId(value!));
    builder.Property(e => e.CreatedBy).HasConversion(id => id!.Value, value => new EmployeeId(value!));
    #endregion


    builder.ToTable("ExpenseCategory");

    builder.Property(e => e.CreatedOn).HasColumnType("datetime");
    builder.Property(e => e.ExpenceName).HasMaxLength(50);
    builder.Property(e => e.UpdatedOn).HasColumnType("datetime");
  }
}
