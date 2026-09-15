using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Application.DTOs.Common;
public class AutorizationUserAttributeModel
{
  public string? Username { get; set; }
  public string? Email { get; set; }
  public string? ClientId { get; set; }
  public string? EmployeeId { get; set; }
  public int RoleId { get; set; }
}
