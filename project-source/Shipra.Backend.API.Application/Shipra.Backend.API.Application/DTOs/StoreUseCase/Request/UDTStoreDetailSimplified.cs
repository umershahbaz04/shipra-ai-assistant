using System.Text;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Application.DTOs.OrderUseCase.HelperOrder.UDTOrder;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Core.CatalougeAggregate;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Helper;
using Shipra.Backend.API.Core.StoresAggregate;

namespace Shipra.Backend.API.Application.DTOs.StoreUseCase.Request;
public class UDTStoreDetailSimplified
{
  public int RowNum { get; set; }
  public string? StoreName { get; set; }
  public string? CompanyName { get; set; }
  public string? Email { get; set; }
  public string? Urls { get; set; }
  public string? LicenseNo { get; set; }

  public string? CustomerServiceNo { get; set; }
  public string? Phone { get; set; }
  public string? CountryCode { get; set; }
  public string? CityName { get; set; }
  public string? State { get; set; }
  public string? Province { get; set; }
  public string? AreaCode { get; set; }
  public string? PinCode { get; set; }
  public string? StreetAddress { get; set; }
  public decimal? Latitude { get; set; }
  public decimal? Longitude { get; set; }
  public string? EmployeeType { get; set; }

  public string? Password { get; set; }
  public DateTime? DateOfBirth { get; set; }



  public static UDTStoreDetailResponse ConvertoStoreDetail(IList<UDTStoreDetailSimplified> dataList, int? countryId, List<Country>? countries, List<City> cities, List<Province> provinces, List<State> states, List<Area> areas, List<PinCode> pinCodes, Catalogue? oCatalogue, List<Store> allStores, List<Employee> allEmployees, Client client)
  {
    var response = new UDTStoreDetailResponse();
    var shipments = new List<CreateUploadStoreResponseModel>();
    var errors = new List<UDTFileUploadError>();

    var i = 0;
    foreach (var x in dataList)
    {
      i++;
      var obj = UDTStoreDetailSimplified.GetCreateStoreCommand(x);
      var error = new UDTFileUploadError();

      obj.RowNum = i;

      #region Index 
      error.Row = i;
      error.IsSuccessed = true;
      #endregion

      #region Customer Name
      if (!string.IsNullOrEmpty(x.StoreName))
      {
        obj.StoreName = x.StoreName;
      }
      else
      {
        error.IsSuccessed = false;
        error.Msg.Add("Store Name is Missing.");
      }
      #endregion 
      #region For Shipper
      if (!string.IsNullOrEmpty(x.Password)) // for extra check like shipper or anything else comes from excel file for now using password if exit then shipper
      {
        if (!string.IsNullOrEmpty(x.Password))
        {
          var validation = Utility.ValidatePassword(x.Password);
          if (validation.IsValid)
          {
            obj.Password = x.Password;

            obj.UserName = GetEmployeeNextCodeByUserName(obj.StoreName, client, oCatalogue, allEmployees);
          }
          else
          {
            error.IsSuccessed = false;
            error.Msg.AddRange(validation.Errors);
          }
        }
        else
        {
          error.IsSuccessed = false;
          error.Msg.Add("Shipper Password is Missing.");
        }
      }
      #endregion

      #region Address
      #region Latitude  
      if (x.Latitude != null && x.Latitude > 0 && x.Longitude != null && x.Longitude > 0)
      {
        var decimalPlaces = 6;
        if (x.Latitude != null && x.Latitude > 0)
        {
          var latitude = x.Latitude!.ToString();
          if (Utility.ValidateLatitude(latitude!))
          {
            var truncatedValue = Math.Truncate(x.Latitude.GetValueOrDefault() * (decimal)Math.Pow(10, decimalPlaces)) / (decimal)Math.Pow(10, decimalPlaces);

            obj.StoreAddress!.Latitude = truncatedValue;
          }
          else
          {
            error.IsSuccessed = false;
            error.Msg.Add("Latitude is invalid.");
          }
        }

        if (x.Longitude != null && x.Longitude > 0)
        {
          var longitude = x.Longitude!.ToString();
          if (Utility.ValidateLongitude(longitude!))
          {
            var truncatedValue = Math.Truncate(x.Longitude.GetValueOrDefault() * (decimal)Math.Pow(10, decimalPlaces)) / (decimal)Math.Pow(10, decimalPlaces);

            obj.StoreAddress!.Longitude = truncatedValue;
          }
          else
          {
            error.IsSuccessed = false;
            error.Msg.Add("Longitude is invalid.");
          }
        }
      }
      #endregion

      Country? country = null;
      #region country,city,state etc
      if (countryId != null && countryId > 0)
      {
        country = countries!.FirstOrDefault(x => x.CountryId == countryId);
        if (country is not null)
        {
          var form = JsonConvert.DeserializeObject<CountryMainForm>(country!.AddressingScheme!);

          foreach (var key in form!.Keys!)
          {
            var field = form!.Fields![key].ToObject<CountryFormField>()!;

            if (form.Fields!.ContainsKey(key))
            {
              #region city 
              if (key == "city")
              {
                if (!string.IsNullOrEmpty(x.CityName))
                {
                  var city = cities.FirstOrDefault(y => y.Name!.Trim().ToLower() == x.CityName.Trim().ToLower() && country.CountryId == y.CountryId);

                  if (city != null)
                  {
                    obj.StoreAddress!.City = city.CityId;
                  }
                  else
                  {
                    error.IsSuccessed = false;
                    error.Msg.Add("Invaild City.");
                  }
                }
                if (field!.Required && string.IsNullOrEmpty(x.CityName))
                {
                  error.IsSuccessed = false;
                  error.Msg.Add("City is Missing.");
                }
              }
              #endregion
              #region province 
              if (key == "province")
              {
                if (!string.IsNullOrEmpty(x.Province))
                {
                  var province = provinces.FirstOrDefault(y => y.Name!.Trim().ToLower() == x.Province.Trim().ToLower() && country.CountryId == y.CountryId);
                  if (province != null)
                  {
                    obj.StoreAddress!.Province = province.ProvinceId;
                  }
                  else
                  {
                    error.IsSuccessed = false;
                    error.Msg.Add("Invaild Province.");
                  }
                }
                if (field.Required && string.IsNullOrEmpty(x.Province))
                {
                  error.IsSuccessed = false;
                  error.Msg.Add("Province is Missing.");
                }
              }
              #endregion
              #region state 
              if (key == "state")
              {
                if (!string.IsNullOrEmpty(x.State))
                {
                  var state = states.FirstOrDefault(y => y.Name!.Trim().ToLower() == x.State.Trim().ToLower() && country.CountryId == y.CountryId);
                  if (state != null)
                  {
                    obj.StoreAddress!.State = state.StateId;
                  }
                  else
                  {
                    error.IsSuccessed = false;
                    error.Msg.Add("Invaild State.");
                  }
                }
                if (field.Required && string.IsNullOrEmpty(x.State))
                {
                  error.IsSuccessed = false;
                  error.Msg.Add("State is Missing.");
                }
              }
              #endregion

              #region area
              else if (key == "area")
              {
                var area = areas != null
                          ? areas.Where(y => y != null &&
                                            y.Code != null &&
                                            x != null &&
                                            x.AreaCode != null &&
                                            y.Code.Trim().ToLower() == x.AreaCode.Trim().ToLower())
                                 .FirstOrDefault()
                          : null;
                if (area != null)
                {
                  obj.StoreAddress!.Area = area.AreaId;
                }

                if (field.Required && string.IsNullOrEmpty(x.AreaCode))
                {
                  error.IsSuccessed = false;
                  error.Msg.Add("Area is Missing.");
                }
              }
              #endregion
              #region pinCode
              else if (key == "pinCode")
              {
                var pincode = pinCodes.Where(y => y.PinCodeValue!.Trim().ToLower() == x.PinCode!.Trim().ToLower()).FirstOrDefault();
                if (pincode != null)
                {
                  obj.StoreAddress!.PinCode = pincode.PinCodeId;
                }

                if (field.Required && string.IsNullOrEmpty(x.PinCode))
                {
                  error.IsSuccessed = false;
                  error.Msg.Add("PinCode is Missing.");
                }
              }
              #endregion
            }
          }

          obj.StoreAddress!.Country = countryId;
        }
      }
      #endregion

      #region MobileNumber
      #region mobile 1
      if (!string.IsNullOrEmpty(x.CustomerServiceNo))
      {
        if (x.CustomerServiceNo.Length <= 20)
        {
          if (country != null)
          {
            var res = Utility.ValidateMobileAndGetNumber(x.CustomerServiceNo, country!.MapCountryCode!.ToUpper());
            if (res.IsValid.GetValueOrDefault())
            {
              obj.CustomerServiceNo = res.PhoneNumber;
            }
            else
            {
              obj.CustomerServiceNo = res.PhoneNumber;

              error.IsSuccessed = false;
              error.Msg.Add("Mobile Number Is Invalid.");
            }
          }
          else
          {
            obj.CustomerServiceNo = x.CustomerServiceNo;
          }
        }
        else
        {
          error.IsSuccessed = false;
          error.Msg.Add("Mobile Number Length is Maximum 20 Characters.");
        }
      }
      else
      {
        error.IsSuccessed = false;
        error.Msg.Add("Mobile Number is Missing.");
      }
      #endregion
      #region mobile 2 
      obj.Phone = x.Phone;
      obj.StoreCode = GetClientNextStoreCode(client, allStores);

      #endregion
      #endregion
      #region StreetAddress
      obj.StoreAddress!.StreetAddress = x.StreetAddress;
      #endregion

      #endregion 
      obj.StoreCompany = x.CompanyName;
      errors.Add(error);
      shipments.Add(obj);
    }
    response.IsSuccessed = errors.All(x => x.IsSuccessed);
    response.UDTStoreDetail = shipments;
    //if (response.IsSuccessed)
    //{
    //}
    response.Errors = errors;
    return response;
  }
  public static string GetClientNextStoreCode(Client? client, List<Store> stores)
  {
    int clientIdentifier = client!.ClientIdentifier ?? 0;
    int storeCount = stores.Count(x => x.ClientId == client!.ClientId);
    string clientStoreCode;

    do
    {
      storeCount++;
      clientStoreCode = $"ST{clientIdentifier}{storeCount}";
    }
    while (stores.Any(x => x.StoreCode == clientStoreCode));

    return clientStoreCode;
  }
  #region new algo 
  public static string GetEmployeeNextCodeByUserName(string? employeeName, Client? client, Catalogue? oCatalogue, List<Employee> allEmployees)
  {
    if (string.IsNullOrWhiteSpace(employeeName))
      throw new ArgumentException("Employee name is required");

    if (oCatalogue == null)
      throw new ArgumentNullException(nameof(oCatalogue));

    int? clientIdentifier = client?.ClientIdentifier ?? 0;

    int empCount = allEmployees.Count + 1;

    string prefix = UtilityHelper.GenerateEmployeeCode(employeeName, true) ?? string.Empty;

    // existing codes lookup (performance fix)
    var existingCodes = new HashSet<string>(
        allEmployees
            .Where(x => !string.IsNullOrEmpty(x.EmployeeCode))
            .Select(x => x.EmployeeCode!)
    );

    string employeeCode;
    int retry = 0;

    do
    {
      var sb = new StringBuilder();
      sb.Append(prefix);
      sb.Append(oCatalogue.DatabaseId);
      sb.Append(clientIdentifier);
      sb.Append(RandomNumGenerator.GetRandom1NumberFrom1To10());
      sb.Append(empCount);

      employeeCode = sb.ToString();

      empCount++;   // avoid same value again
      retry++;

      if (retry > 10)
        throw new InvalidOperationException("Unable to generate unique employee code.");

    } while (existingCodes.Contains(employeeCode));

    return employeeCode;
  }
  #endregion
  public static CreateUploadStoreResponseModel GetCreateStoreCommand(UDTStoreDetailSimplified x)
  {
    return new CreateUploadStoreResponseModel
    {
      RowNum = 0,
      StoreId = 0,
      StoreName = "",
      StoreCompany = "",
      CustomerServiceNo = "",
      Phone = "",
      Email = "",
      Urls = "",
      LicenseNo = "",
      StoreImage = "",
      StoreAddress = new AddressResponseDTO()
      {
        StreetAddress = ""
      }
    };
  }

}
