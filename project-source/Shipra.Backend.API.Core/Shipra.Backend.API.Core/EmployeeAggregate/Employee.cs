using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.Constants;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Helper;

namespace Shipra.Backend.API.Core.EmployeeAggregate;
public class Employee
{
  public EmployeeId? EmployeeId { get; private set; }
  public ClientId? ClientId { get; private set; }
  public string? EmployeeCode { get; private set; }
  public string? EmployeeName { get; private set; }
  public int? GenderId { get; private set; }
  public DateTime? DateOfBirth { get; private set; }
  public string? PhoneNo { get; private set; }
  public int? ClientUserRoleId { get; private set; }
  public string? MobileNo { get; private set; }
  public string? WorkEmail { get; private set; }
  public int? EmployeeTypeId { get; set; }
  public int? UserTypeId { get; set; }
  //public int? CountryId { get; private set; }
  //public int? RegionId { get; private set; }
  //public int? CityId { get; private set; }
  public int? SaleChannelConfigId { get; private set; }
  //public string? AddressLine1 { get; private set; }
  //public string? AddressLine2 { get; private set; }
  public string? EmployeeImage { get; private set; }
  //public string? Zip { get; private set; }
  //public decimal? Latitude { get; private set; }
  //public decimal? Longitude { get; private set; }
  public bool? IsClient { get; set; }
  public DateTime? CreatedOn { get; private set; }
  public EmployeeId? CreatedBy { get; private set; }
  public DateTime? UpdatedOn { get; private set; }
  public EmployeeId? UpdatedBy { get; private set; }
  public bool? Active { get; private set; }

  public static Employee CreateEmployee(EmployeeId employeeId, ClientId clientId, string employeeCode, string? employeeName, int? genderId, DateTime? dateOfBirth, string? phone, string? mobile, string? workEmail, string? employeeImage, int? userRoleId, int? userTypeId, EmployeeId createdBy, int employeeTypeId = (int)EnumEmployeeType.Employee, bool? isClient = false, int? saleChannelConfigId = null)
  {
    return new Employee()
    {
      EmployeeId = employeeId,
      GenderId = genderId,
      ClientId = clientId,
      EmployeeCode = employeeCode,
      EmployeeName = employeeName,
      DateOfBirth = dateOfBirth,
      PhoneNo = UtilityHelper.CleanPhoneNumber(phone),
      MobileNo = UtilityHelper.CleanPhoneNumber(mobile),
      WorkEmail = workEmail,
      //CountryId = countryId,
      //RegionId = regionId,
      //CityId = cityId,
      EmployeeTypeId = employeeTypeId,
      UserTypeId = userTypeId,
      SaleChannelConfigId = saleChannelConfigId,
      //AddressLine1 = addressLine1,
      //AddressLine2 = addressLine2,
      EmployeeImage = employeeImage,
      ClientUserRoleId = userRoleId, 
      IsClient = isClient, 
      CreatedOn = DateTime.UtcNow,
      CreatedBy = createdBy,
      Active = true
    };
  }
  public void EnableDisableEmployee(bool? isActive,EmployeeId updatedBy)
  {
    Active = isActive;
    UpdatedOn = DateTime.UtcNow;
    UpdatedBy = updatedBy;
  }

  public void UpdateEmployee(string? employeeName, string? phone, string? mobile, string? workEmail, string? employeeImage, int genderId,int? userRoleId, DateTime? dateOfBirth, EmployeeId? userId, int employeeTypeId = (int)EnumEmployeeType.Employee)
  {
    EmployeeName = employeeName;
    PhoneNo = UtilityHelper.CleanPhoneNumber(phone);
    MobileNo = UtilityHelper.CleanPhoneNumber(mobile); 
    WorkEmail = workEmail;
    EmployeeImage = employeeImage; 
    GenderId = genderId;
    DateOfBirth = dateOfBirth;
    ClientUserRoleId = userRoleId;
    UpdatedBy = userId;
    UpdatedOn = DateTime.UtcNow;
  }

  public void UpdateSaleChannel(string salePersonName, string phoneNo, string address, EmployeeId? updatedBy)
  {
    EmployeeName = salePersonName;
    PhoneNo = phoneNo;
    //AddressLine1 = address;
    UpdatedBy = updatedBy;
    UpdatedOn = DateTime.UtcNow;
  }

  public void UpdateSaleChannelConfigId(int? saleChannelConfigId, EmployeeId? updatedBy)
  {
    SaleChannelConfigId = saleChannelConfigId;
    UpdatedBy = updatedBy;
    UpdatedOn = DateTime.UtcNow;
  }

}

public sealed record EmployeeId(Guid Value)
{
  public static EmployeeId New => new(Guid.NewGuid());
}
