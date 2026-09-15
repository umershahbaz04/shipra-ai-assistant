//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using Shipra.Backend.API.Core.AwsAggregate;
//using Shipra.Backend.API.Core.ClientAggregate;

//namespace Shipra.Backend.API.Infrastructure.Data.Config;
//public class AwsCognitoCredentialConfiguration : IEntityTypeConfiguration<AwsCognitoCredential>
//{ 
//  public void Configure(EntityTypeBuilder<AwsCognitoCredential> builder)
//  {
//    builder.HasKey(e => e.AwsCognitoCredentialId);

//    builder.ToTable("AwsCognitoCredential");

//    #region conversion
//    builder.Property(e => e.AwsCognitoCredentialId).HasConversion(Id => Id!.Value, value => new AwsCognitoCredentialId(value!));
//    builder.Property(e => e.ClientId).HasConversion(Id => Id!.Value, value => new ClientId(value!));
//    #endregion

//    builder.Property(e => e.AwsCognitoCredentialId).ValueGeneratedNever();
//    builder.Property(e => e.GroupName).HasMaxLength(50);
//    builder.Property(e => e.PoolName).HasMaxLength(50);
//    builder.Property(e => e.UserName).HasMaxLength(50);
//    builder.Property(e => e.UserPoolClientId)
//        .HasMaxLength(10)
//        .IsFixedLength();
//    builder.Property(e => e.UserPoolClientSecretId)
//        .HasMaxLength(10)
//        .IsFixedLength();
//    builder.Property(e => e.UserPoolId).HasMaxLength(50);
//  }
//}
