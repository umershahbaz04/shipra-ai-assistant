using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.UserRoleAndPermissionAggregate;
public class PermissionGroupLookup
{
  public int PermissionGroupId { get; set; }
  public string? GroupName { get; set; }
  public string? GroupParent { get; set; }
  public bool? Active { get; private set; }

  public static PermissionGroupLookup Create(string groupName, string groupParent)
  {
    return new PermissionGroupLookup
    {
      GroupName = groupName,
      GroupParent = groupParent,
      Active = true
    };
  }
}
