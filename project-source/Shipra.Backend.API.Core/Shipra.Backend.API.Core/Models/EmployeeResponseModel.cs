using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.Models;
public class EmployeeResponseModel
{ 
  public string? EmployeeName { get; set; }
  public string? EmployeeCode { get; set; }
  public string? EmployeeImage { get; set; }
  public string? Mobile { get; set; }
  public string? Phone { get; set; } 
  public string? Email { get; set; }
}
