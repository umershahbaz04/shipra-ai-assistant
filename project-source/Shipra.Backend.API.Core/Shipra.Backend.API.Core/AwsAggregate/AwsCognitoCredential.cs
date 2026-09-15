using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.ClientAggregate;

namespace Shipra.Backend.API.Core.AwsAggregate;
public class AwsCognitoCredential
{
  public AwsCognitoCredentialId? AwsCognitoCredentialId { get; set; } 
  public ClientId? ClientId { get; private set; } 
  public string? UserPoolId { get; private set; } 
  public string? UserPoolClientId { get; private set; } 
  public string? UserPoolClientSecretId { get; private set; } 
  public string? PoolName { get; private set; } 
  public string? GroupName { get; private set; }
  public string? UserName { get; private set; }

  public static AwsCognitoCredential CreateAwsCognitoCredential(ClientId clientId, string? userPoolId, string? userPoolClientId, string? userPoolClientSecretId, string groupName, string poolName,string UserName)
  {
    return new AwsCognitoCredential()
    {
      AwsCognitoCredentialId = AwsCognitoCredentialId.New,
       ClientId = clientId,
       UserPoolId = userPoolId,
       UserPoolClientId = userPoolClientId,
       UserPoolClientSecretId = userPoolClientSecretId,
       GroupName = groupName,
       PoolName = poolName,
       UserName = UserName
    };
  }
}
public sealed record AwsCognitoCredentialId(Guid Value)
{
  public static AwsCognitoCredentialId New => new(Guid.NewGuid());
}
