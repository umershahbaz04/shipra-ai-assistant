using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Application.DTOs.PermissionUseCase;
public class AddUpdateClientRolePermissionGroupRequestModel
{
  public int ClientRolePgid { get; set; }
  public int? RolePermissionGroupId { get; set; }
  public bool? HavePermission { get; set; } 
}
