using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Constants;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Helper;

namespace Shipra.Backend.API.Core.StoresAggregate;
public class Store
{
  public Store() { }
  public int StoreId { get; set; }
  public ClientId? ClientId { get; set; }
  public string? StoreCode { get; set; }
  public string? StoreName { get; set; }
  //public string? StoreAddress { get; set; }
  public string? StoreCompany { get; set; }
  //public int? CountryId { get; set; }
  //public int? RegionId { get; set; }
  //public int? CityId { get; set; }
  //public string? Area { get; set; }
  //public string? StreetAddress { get; set; }
  //public string? Zip { get; set; }
  public string? CustomerServiceNo { get; set; } 
  public string? Phone { get; set; }
  public string? Email { get; set; }
  public string? Urls { get; set; }
  public string? StoreImage { get; set; }
  public EmployeeId? CreatedBy { get; set; }
  public DateTime? CreatedOn { get; set; }
  public EmployeeId? UpdatedBy { get; set; }
  public DateTime? UpdatedOn { get; set; }
  public string? LicenseNo { get; set; }
  public bool? Active { get; set; }
  public bool? IsDefault { get; set; } 
  public static Store CreateStore(ClientId? clientId, string? storeName, string storeCode, string? storeCompany, string? customerServiceNo, string? phone, string? email, string? urls, string? storeImage,string? licenseNo, EmployeeId? createdBy, bool? isdefault = false)
  {
    return new Store()
    {
      StoreId = new int(),
      ClientId = clientId,
      StoreName = storeName,
      StoreCode = storeCode,
      //StoreAddress = storeAddress,
      //Area = area,
      StoreCompany = storeCompany,
      //CountryId = countryId,
      //RegionId = regionId,
      //CityId = cityId,
      //StreetAddress = streetAddress,
      //Zip = zip,
      LicenseNo = licenseNo, 
      //Latitude = latitude,
      //Longitude = longitude,
      CustomerServiceNo = UtilityHelper.CleanPhoneNumber(customerServiceNo),
      Phone = UtilityHelper.CleanPhoneNumber(phone),
      Email = email,
      Urls = urls,
      StoreImage = storeImage,
      CreatedBy = createdBy,
      CreatedOn = DateTime.UtcNow,
      Active = true,
      IsDefault = isdefault
    };
  }

  public void UpdateStore(string? storeName,string? storeCompany, string? customerServiceNo, string? phone, string? email, string? urls, string? storeImage, string? licenseNo, EmployeeId? updatedBy)
  {
    StoreName = storeName; 
    StoreCompany = storeCompany; 
    CustomerServiceNo = UtilityHelper.CleanPhoneNumber(customerServiceNo); 
    Phone = UtilityHelper.CleanPhoneNumber(phone);
    Email = email;
    Urls = urls;
    StoreImage = storeImage;
    LicenseNo = licenseNo;
    UpdatedBy = updatedBy;
    UpdatedOn = DateTime.UtcNow;
  }
  public void DisableStore(EmployeeId employeeId)
  {
    Active = false;
    UpdatedOn = DateTime.UtcNow;
    UpdatedBy = employeeId;
  }

  public void EnableStore(EmployeeId employeeId)
  {
    Active = true;
    UpdatedOn = DateTime.UtcNow;
    UpdatedBy = employeeId;
  }
  public static Store AddDefault()
  {
    return new Store()
    {
      StoreId = 0,
      StoreName = ShipraConstants.DropDownPlaceHolderName
    };
  }
}
