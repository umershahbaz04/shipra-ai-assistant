using Shipra.Backend.API.Application.DTOs.Common.Response;

namespace Shipra.Backend.API.Application.DTOs.EmployeeUseCase;
public class EmployeResponseModel
{
  public string? EmployeeId { get;  set; } 
  public string? EmployeeCode { get;  set; }
  public string? EmployeeName { get;  set; }
  public int? GenderId { get;  set; }
  public int? StoreId { get;  set; }
  public DateTime? DateOfBirth { get;  set; }
  public string? PhoneNo { get;  set; } 
  public int? ClientUserRoleId { get;  set; }
  public string? MobileNo { get;  set; }
  public string? WorkEmail { get;  set; }
  public int? EmployeeTypeId { get; set; } 
  public int? UserTypeId { get; set; }
  public int? SaleChannelConfigId { get;  set; } 
  public string? EmployeeImage { get;  set; } 
  public bool? IsClient { get; set; } 
  public bool? Active { get;  set; }
  public AddressResponseDTO? Address { get; set; }
}
