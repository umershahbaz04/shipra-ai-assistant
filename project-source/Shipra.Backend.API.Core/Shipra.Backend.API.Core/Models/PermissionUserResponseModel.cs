using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.Models;
public class PermissionUserResponseModel
{
  public string? ControllerName { get; set; }
  public bool GroupAssigned { get; set; }
  public string? ActionName { get; set; }
  public string? RoleName { get; set; }
  public string? GroupName { get; set; }
  public string? GroupParent { get; set; }
}
