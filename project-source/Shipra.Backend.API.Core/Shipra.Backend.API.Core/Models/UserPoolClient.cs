using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.Models;
public partial class UserPoolClientResponseModel
{
  public long UserPoolClientId { get; set; } 
  public string? ClientName { get; set; } 
  public string? UserPoolId { get; set; } 
  public string? PoolClientId { get; set; }
  public Guid? TenantClientId { get; set; } 
  public string? UserPoolClientSecret { get; set; }
  public bool? IsCodeConfirm { get; set; }
  public string? Region { get; set; }
}
