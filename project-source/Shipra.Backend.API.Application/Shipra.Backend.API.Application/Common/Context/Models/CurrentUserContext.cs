using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Application.Common.Context.Models;

public class CurrentUserContext
{
  public CurrentUserContext()
  {
    NotMappedRoles = new List<string>();
  }
  //id is also clinet id (tenantid) in string formate
  public string? ClientIdStr { get; set; }
  public ClientId? ClientId { get; set; }
  public EmployeeId? EmployeeId { get; set; }
  public int? RoleId { get; set; }
  public string? EmployeeIdStr { get; set; }
  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public string? UserName { get; set; }
  public string? AccessToken { get; set; }
  public bool VerifiedEmail { get; set; }
  //user type customer/administrator
  public int UserTypeId { get; set; }
  public virtual List<string> NotMappedRoles { get; set; }
  public int? VendorId { get; set; }
  public int? VendorUserId { get; set; }
  public int? EnvironmentTypeId { get; set; }
}
