//using Shipra.Backend.API.Core.ContributorAggregate;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;

//namespace Shipra.Backend.API.Infrastructure.Data.Config;

//public class ContributorConfiguration : IEntityTypeConfiguration<Contributor>
//{
//  public void Configure(EntityTypeBuilder<Contributor> builder)
//  {
//    builder.HasNoKey();
//    builder.Property(p => p.Name)
//        .HasMaxLength(100)
//        .IsRequired();
//  }
//}
