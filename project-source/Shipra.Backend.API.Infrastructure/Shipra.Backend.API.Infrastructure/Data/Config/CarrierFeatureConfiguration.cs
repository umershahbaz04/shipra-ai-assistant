using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Config;
public class CarrierFeatureConfiguration : IEntityTypeConfiguration<CarrierFeature>
{
  public void Configure(EntityTypeBuilder<CarrierFeature> entity)
  {
    entity.HasKey(e => e.CarrierFeatureId);
    #region conversion
    entity.Property(e => e.CarrierFeatureId).HasConversion(carrierFeatureId => carrierFeatureId!.Value, value => new CarrierFeatureId(value));
    entity.Property(e => e.CreatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    entity.Property(e => e.UpdatedBy).HasConversion(uuid => uuid!.Value, value => new EmployeeId(value));
    #endregion
    entity.ToTable("CarrierFeature");
    entity.Property(e => e.CarrierFeatureId).ValueGeneratedNever();
    entity.Property(e => e.Active)
        .IsRequired()
        .HasDefaultValueSql("((1))");
    entity.Property(e => e.CreatedOn).HasColumnType("datetime");
    entity.Property(e => e.Feature).HasMaxLength(150);
    entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
  }
}

