//using Shipra.Backend.API.Core.ProjectAggregate;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;

//namespace Shipra.Backend.API.Infrastructure.Data.Config;

//public class ProjectConfiguration : IEntityTypeConfiguration<Project>
//{
//  public void Configure(EntityTypeBuilder<Project> builder)
//  {
//    builder.HasNoKey();

//    builder.Property(p => p.Name)
//        .HasMaxLength(100)
//        .IsRequired();

//    builder.Property(p => p.Priority)
//      .HasConversion(
//          p => p.Value,
//          p => PriorityStatus.FromValue(p));
//  }
//}
