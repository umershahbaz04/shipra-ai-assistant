using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.ClientAggregate;
public class ClientRolePermissionGroup
{
  /// <summary>
  /// Client Role Permission GroupId
  /// </summary>
  public int ClientRolePgid { get; private set; }
  public ClientId? ClientId { get; private set; }
  public int? ClientUserRoleId { get; private set; }
  public int? PermissionGroupId { get; private set; }
  public bool? Active { get; private set; }

  public static ClientRolePermissionGroup Create(int clientUserRoleId, int? rolePermissionGroupId, ClientId clientId)
  {
    return new ClientRolePermissionGroup
    {
      ClientUserRoleId = clientUserRoleId,
      PermissionGroupId = rolePermissionGroupId,
      ClientId = clientId,
      Active = true
    };
  }
}
