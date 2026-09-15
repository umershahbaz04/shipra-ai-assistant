
using System;
using System.Text;
using System.Xml.Linq;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Functions;
using Shipra.Backend.API.Application.DTOs.ProductUseCase.Request;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrder;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.ProductAggregate;
using Shipra.Backend.API.Core.SaleChannelConfigAggregate;
using Shipra.Backend.API.Core.StoresAggregate;
using Utility = Shipra.Backend.API.Application.Helpers.Utility;

namespace Shipra.Backend.API.Application.DTOs.OrderUseCase.HelperOrder.UDTOrder;

public class UDTOrderDetailSimplified : UDTOrderDetailSimplifiedCommon
{
  public string? Products { get; set; }
  public static UDTOrderDetailResponse ConvertoRegularOrderDetail(IList<UDTOrderDetailSimplified> dataList, int? countryId, List<Country>? countries, List<City> cities, List<Province> provinces, List<State> states, List<Area> areas, List<PinCode> pinCodes, List<PaymentMethodLookup>? paymentMethods, List<ProductStation>? allProductStations, List<ClientOrderBox> clientOrderBoxes, int defaultStationId, IMapper _mapper, List<Store> allStores, List<Employee> allEmployees, List<SaleChannelConfig> allSaleChannelConfigs, Client client)
  {
    var response = new UDTOrderDetailResponse();
    var shipments = new List<CreateUploadOrderResponseModel>();
    var errors = new List<UDTFileUploadError>();

    var i = 0;
    foreach (var x in dataList)
    {
      i++;
      var obj = UDTOrderDetailSimplifiedFullfilable.GetCreateOrderCommand(x);
      var error = new UDTFileUploadError();

      #region Index 
      error.Row = i;
      error.IsSuccessed = true;
      #endregion

      #region Amount
      if (x.Amount > 0)
      {
        obj.Amount = x.Amount;
      }
      else
      {
        obj.Amount = 0;
      }
      #endregion

      #region Payment Method
      if (!string.IsNullOrEmpty(x.PaymentMethod))
      {
        var paymentMethod = paymentMethods!.Where(y => y.Code == x.PaymentMethod.Trim()).FirstOrDefault();

        if (paymentMethod != null)
        {
          var PaymentMethodId = Enum.Parse<EnumPaymentMethod>(x.PaymentMethod);
          obj.PaymentMethodId = (int)PaymentMethodId;
        }
        else
        {
          //if no payment method define
          obj.PaymentMethodId = (int)EnumPaymentMethod.COD;
        }
      }
      else
      {
        error.IsSuccessed = false;
        error.Msg.Add("Payment Method is Missing.");
      }
      #endregion 
      #region Customer Name
      if (!string.IsNullOrEmpty(x.CustomerName))
      {
        obj.OrderAddress!.CustomerName = x.CustomerName;
      }
      else
      {
        error.IsSuccessed = false;
        error.Msg.Add("Customer Name is Missing.");
      }
      #endregion
      #region station check
      if (!string.IsNullOrEmpty(x.StationCode))
      {
        var selectedStation = allProductStations!.FirstOrDefault(j => j.StationCode!.Trim().ToLower() == x.StationCode.Trim().ToLower());
        if (selectedStation is not null)
        {
          obj.StationId = selectedStation.ProductStationId;
        }
        else
        {
          //we will replace with default id
          obj.StationId = defaultStationId;
        }
      }
      else
      {
        //we will replace with default id
        obj.StationId = defaultStationId;
      }
      #endregion
      #region store check 
      if (!string.IsNullOrEmpty(x.StoreCode))
      {
        var selectedStore = allStores!.FirstOrDefault(j => j.StoreCode!.Trim().ToLower() == x.StoreCode.Trim().ToLower());
        if (selectedStore is not null)
        {
          obj.StoreId = selectedStore.StoreId;
        }
      }
      #endregion
      #region sale person check
      if (!string.IsNullOrEmpty(x.SalePerson))
      {
        var selectedSaleChannelConfig = allEmployees!.FirstOrDefault(j => (j.EmployeeName!.Trim().ToLower() == x.SalePerson.Trim().ToLower() || j.EmployeeCode!.Trim().ToLower() == x.SalePerson.Trim().ToLower()) && j.SaleChannelConfigId.GetValueOrDefault() > 0);
        if (selectedSaleChannelConfig is not null)
        {
          var scAgainnstStore = allSaleChannelConfigs.FirstOrDefault(j => j.SaleChannelName!.Trim().ToLower() == x.SalePerson.Trim().ToLower());
          if (scAgainnstStore is not null)
          {
            obj.StoreId = scAgainnstStore.StoreId;
          }

          //scAgainnstStore = allSaleChannelConfigs.FirstOrDefault(x => x.StoreId == obj.StoreId);

          if (scAgainnstStore is not null)
          {
            obj.SaleChannelConfigId = selectedSaleChannelConfig.SaleChannelConfigId;
            obj.SaleChannelLookupId = (int)EnumSaleChannelLookup.SalePerson;
          }
          else
          {
            error.IsSuccessed = false;
            error.Msg.Add($"Sale person not found against store {x.StoreCode}.");
          }
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

            obj.OrderAddress!.Latitude = truncatedValue;
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

            obj.OrderAddress!.Longitude = truncatedValue;
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
                  var cityNameLower = x.CityName.Trim().ToLower();
                  var city = cities.FirstOrDefault(y => 
                      (y.Name != null && y.Name.Trim().ToLower() == cityNameLower) ||
                      (y.Code != null && y.Code.Trim().ToLower() == cityNameLower)
                  );

                  if (city != null)
                  {
                    obj.OrderAddress!.City = city.CityId;
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
                    obj.OrderAddress!.Province = province.ProvinceId;
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
                    obj.OrderAddress!.State = state.StateId;
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
                  obj.OrderAddress!.Area = area.AreaId;
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
                  obj.OrderAddress!.PinCode = pincode.PinCodeId;
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

          obj.OrderAddress!.Country = countryId;
        }
      }
      #endregion

      #region MobileNumber
      #region mobile 1
      if (!string.IsNullOrEmpty(x.MobileNumber))
      {
        if (x.MobileNumber.Length <= 20)
        {
          if (country != null)
          {
            var res = Utility.ValidateMobileAndGetNumber(x.MobileNumber, country!.MapCountryCode!.ToUpper());
            if (res.IsValid.GetValueOrDefault())
            {
              obj.OrderAddress!.Mobile1 = res.PhoneNumber;
            }
            else
            {
              obj.OrderAddress!.Mobile1 = res.PhoneNumber;

              error.IsSuccessed = false;
              error.Msg.Add("Mobile Number Is Invalid.");
            }
          }
          else
          {
            obj.OrderAddress!.Mobile1 = x.MobileNumber;
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
      //check validity if need
      //if (!string.IsNullOrEmpty(x.MobileNumber2))
      //{
      //  if (country != null)
      //  {
      //    ValidatePhoneResponseModel res = Utility.ValidateMobileAndGetNumber(x.MobileNumber2, country!.MapCountryCode!.ToUpper());
      //    if (res.IsValid.GetValueOrDefault())
      //    {
      //      obj.OrderAddress!.Mobile2 = res.PhoneNumber;
      //    }
      //    else
      //    {
      //      error.IsSuccessed = false;
      //      error.Msg.Add("Mobile Number 2 Is Invalid.");
      //    }
      //  }
      //  else
      //  {
      //    obj.OrderAddress!.Mobile2 = x.MobileNumber2;
      //  }
      //}
      obj.OrderAddress!.Mobile2 = x.MobileNumber2 == x.MobileNumber ? "" : x.MobileNumber2;

      #endregion
      #endregion
      #region StreetAddress
      obj.OrderAddress!.StreetAddress = x.StreetAddress;
      #endregion

      #endregion

      #region product
      int pieces = x.NoOfPieces.GetValueOrDefault(1);
      if (pieces <= 0) pieces = 1;

      for (int pieceIndex = 0; pieceIndex < pieces; pieceIndex++)
      {
        obj.OrderItems!.Add(new OrderItemModel
        {
          ProductId = "",
          ProductVariantId = 0,
          Price = (pieceIndex == 0) ? x.Amount : 0,
          Description = x.Description,
          Remarks = "",
          Quantity = 1,
          Discount = 0
        });
      }
      #endregion

      #region order box
      ClientOrderBox? clientOrderBox = null;
      if (string.IsNullOrWhiteSpace(x.BoxName))
      {
        clientOrderBox = clientOrderBoxes.FirstOrDefault(y => y.IsDefault == true);
      }
      else
      {
        var boxName = x.BoxName!.Trim();
        clientOrderBox = clientOrderBoxes.FirstOrDefault(y => !string.IsNullOrWhiteSpace(y.BoxName) && string.Equals(boxName, y.BoxName.Trim(), StringComparison.OrdinalIgnoreCase));

        if (clientOrderBox is null)
        {
          error.IsSuccessed = false;
          error.Msg.Add($"Box with name:{x.BoxName} not found.");
        }
      }
      if (clientOrderBox is not null)
      {
        var mapData = _mapper.Map<ClientOrderBoxResponseModel>(clientOrderBox);
        obj.OrderBoxs!.Add(mapData);
      }
      #endregion

      //if (obj.ItemsCount == 0)
      //  {
      //    error.IsSuccessed = false;
      //    error.Msg.Add("Number of pieces must be greater then 0.");
      //  }

      obj.Remarks = x.Remarks;
      obj.Description = x.Description;
      obj.RefNo = x.RefNo;
      obj.ItemsCount = x.NoOfPieces.GetValueOrDefault(1) <= 0 ? 1 : x.NoOfPieces.GetValueOrDefault(1);
      errors.Add(error);
      shipments.Add(obj);
    }
    response.IsSuccessed = errors.All(x => x.IsSuccessed);
    response.UDTOrderDetail = shipments;
    //if (response.IsSuccessed)
    //{
    //}
    response.Errors = errors;
    return response;
  }


}
public class UDTOrderDetailSimplifiedFullfilable : UDTOrderDetailSimplifiedCommon
{
  public string? Products { get; set; }

  public static UDTOrderDetailResponse ConvertoFullfilableOrderDetail(IList<UDTOrderDetailSimplified> dataList, int? countryId, List<Country>? countries, List<City> cities, List<Province> provinces, List<State> states, List<Area> areas, List<PinCode> pinCodes, List<PaymentMethodLookup>? paymentMethods, dynamic? products, List<ProductStation>? allProductStations, int defaultStationId, List<ClientOrderBox> clientOrderBoxes, IMapper _mapper, List<Employee> allEmployees, List<SaleChannelConfig> allSaleChannelConfigs)
  {
    var response = new UDTOrderDetailResponse();
    var shipments = new List<CreateUploadOrderResponseModel>();
    var errors = new List<UDTFileUploadError>();

    var i = 0;
    foreach (var x in dataList)
    {
      i++;
      var obj = GetCreateOrderCommand(x);
      var error = new UDTFileUploadError();

      #region Index 
      error.Row = i;
      error.IsSuccessed = true;
      #endregion

      #region Amount
      if (x.Amount > 0)
      {
        obj.Amount = x.Amount;
      }
      else
      {
        obj.Amount = 0;
      }
      #endregion

      #region Payment Method
      if (!string.IsNullOrEmpty(x.PaymentMethod))
      {
        var paymentMethod = paymentMethods!.Where(y => y.Code == x.PaymentMethod.Trim()).FirstOrDefault();

        if (paymentMethod != null)
        {
          var PaymentMethodId = Enum.Parse<EnumPaymentMethod>(x.PaymentMethod);
          obj.PaymentMethodId = (int)PaymentMethodId;
        }
        else
        {
          error.IsSuccessed = false;
          error.Msg.Add("Invaild Payment Method Code!.");
        }
      }
      else
      {
        //if no payment method define
        obj.PaymentMethodId = (int)EnumPaymentMethod.COD;
      }
      #endregion 
      #region Customer Name
      if (!string.IsNullOrEmpty(x.CustomerName))
      {
        obj.OrderAddress!.CustomerName = x.CustomerName;
      }
      else
      {
        error.IsSuccessed = false;
        error.Msg.Add("Customer Name is Missing.");
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

            obj.OrderAddress!.Latitude = truncatedValue;
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

            obj.OrderAddress!.Longitude = truncatedValue;
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

            Console.WriteLine($"Country: {form.Country}");
            if (form.Fields!.ContainsKey(key))
            {
              #region city 
              if (key == "city")
              {
                if (!string.IsNullOrEmpty(x.CityName))
                {
                  var cityNameLower = x.CityName.Trim().ToLower();
                  var city = cities.FirstOrDefault(y => 
                      (y.Name != null && y.Name.Trim().ToLower() == cityNameLower) ||
                      (y.Code != null && y.Code.Trim().ToLower() == cityNameLower)
                  );

                  if (city != null)
                  {
                    obj.OrderAddress!.City = city.CityId;
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
                    obj.OrderAddress!.Province = province.ProvinceId;
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
                    obj.OrderAddress!.State = state.StateId;
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
                  obj.OrderAddress!.Area = area.AreaId;
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
                  obj.OrderAddress!.PinCode = pincode.PinCodeId;
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

          obj.OrderAddress!.Country = countryId;
        }
      }
      #endregion

      #region MobileNumber
      #region mobile 1
      if (!string.IsNullOrEmpty(x.MobileNumber))
      {
        if (x.MobileNumber.Length <= 20)
        {
          if (country != null)
          {
            var res = Utility.ValidateMobileAndGetNumber(x.MobileNumber, country!.MapCountryCode!.ToUpper());
            if (res.IsValid.GetValueOrDefault())
            {
              obj.OrderAddress!.Mobile1 = res.PhoneNumber;
            }
            else
            {
              obj.OrderAddress!.Mobile1 = res.PhoneNumber;

              error.IsSuccessed = false;
              error.Msg.Add("Mobile Number Is Invalid.");
            }
          }
          else
          {
            obj.OrderAddress!.Mobile1 = x.MobileNumber;
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
      obj.OrderAddress!.Mobile2 = x.MobileNumber2 == x.MobileNumber ? "" : x.MobileNumber2;
      #endregion
      #endregion
      #region StreetAddress
      obj.OrderAddress!.StreetAddress = x.StreetAddress;
      #endregion

      #endregion

      #region product
      #region station check
      if (!string.IsNullOrEmpty(x.StationCode))
      {
        var selectedStation = allProductStations!.FirstOrDefault(j => j.StationCode!.Trim().ToLower() == x.StationCode.Trim().ToLower());
        if (selectedStation is not null)
        {
          obj.StationId = selectedStation.ProductStationId;
        }
        else
        {
          //we will replace with default id
          obj.StationId = defaultStationId;
        }
      }
      else
      {
        //we will replace with default id
        obj.StationId = defaultStationId;
      }
      #endregion
      if (!string.IsNullOrEmpty(x.Products))
      {
        var fileProductWithErr = new List<string>();
        var validatedFileProduct = new List<string>();
        foreach (var productStr in x.Products.Split(','))
        {
          var splitedProd = productStr.Split('_');
          if (splitedProd.Length == 4)
          {
            var castedList = (IEnumerable<dynamic>)products!;
            var sku = splitedProd.First();
            var qty = int.Parse(splitedProd[1]);
            var itemDiscount = Convert.ToDecimal(splitedProd[2]);
            var amount = Convert.ToDecimal(splitedProd[3]);

            //filter product against station and sku
            var product = castedList.FirstOrDefault(product => product.SKU == sku && product.ProductStationId == obj.StationId);

            if (product != null)
            {
              obj.OrderItems!.Add(new OrderItemModel
              {
                ProductId = product.ProductId,
                ProductVariantId = product.ProductVariantId,
                StockSku = sku,
                Price = amount,
                Description = "",
                Remarks = "",
                Quantity = qty,
                Discount = itemDiscount
              });
              obj.StoreId = product.StoreId;
              validatedFileProduct.Add(productStr);
            }
            else
            {
              fileProductWithErr.Add(productStr.Trim());
            }
          }
          else
          {
            error.IsSuccessed = false;
            error.Msg.Add("Product format is not correct. " + productStr);
          }
        }
        //#region sale person check
        //if (!string.IsNullOrEmpty(x.SalePerson))
        //{
        //  var selectedSaleChannelConfig = allEmployees!.FirstOrDefault(j => (j.EmployeeName!.Trim().ToLower() == x.SalePerson.Trim().ToLower() || j.EmployeeCode!.Trim().ToLower() == x.SalePerson.Trim().ToLower()) && j.SaleChannelConfigId.GetValueOrDefault() > 0);
        //  if (selectedSaleChannelConfig is not null)
        //  {
        //    var scAgainnstStore = allSaleChannelConfigs.FirstOrDefault(j => j.SaleChannelName!.Trim().ToLower() == x.SalePerson.Trim().ToLower());
        //    if (scAgainnstStore is not null)
        //    {
        //      obj.StoreId = scAgainnstStore.StoreId;
        //    }

        //    //scAgainnstStore = allSaleChannelConfigs.FirstOrDefault(x => x.StoreId == obj.StoreId);

        //    if (scAgainnstStore is not null)
        //    {
        //      obj.SaleChannelConfigId = selectedSaleChannelConfig.SaleChannelConfigId;
        //      obj.SaleChannelLookupId = (int)EnumSaleChannelLookup.SalePerson;
        //    }
        //    else
        //    {
        //      error.IsSuccessed = false;
        //      error.Msg.Add($"Sale person not found against store {x.StoreCode}.");
        //    }
        //  }
        //}
        //#endregion

        #region sale person check
        if (!string.IsNullOrEmpty(x.SalePerson))
        {
          var selectedSaleChannelConfig = allEmployees!.FirstOrDefault(j => (j.EmployeeName!.Trim().ToLower() == x.SalePerson.Trim().ToLower() || j.EmployeeCode!.Trim().ToLower() == x.SalePerson.Trim().ToLower()) && j.SaleChannelConfigId.GetValueOrDefault() > 0);
          if (selectedSaleChannelConfig is not null)
          {
            var scAgainnstStore = allSaleChannelConfigs.FirstOrDefault(x => x.StoreId == x.StoreId);
            if (scAgainnstStore is not null)
            {
              obj.SaleChannelConfigId = selectedSaleChannelConfig.SaleChannelConfigId;
              obj.SaleChannelLookupId = (int)EnumSaleChannelLookup.SalePerson;
            }
            else
            {
              error.IsSuccessed = false;
              error.Msg.Add($"Sale person not found against store {x.StoreCode}.");
            }
          }
        }
        #endregion
        if (fileProductWithErr.Count > 0)
        {
          var errStr = string.Join(',', fileProductWithErr.Select(x => x));
          error.IsSuccessed = false;
          var err = $"Products not found against. {Environment.NewLine} {errStr}";
          error.Msg.Add(err);
        }

      }
      else
      {
        error.IsSuccessed = false;
        error.Msg.Add("Products is Missing.");
      }
      #endregion
      #region order box
      ClientOrderBox? clientOrderBox = null;
      if (string.IsNullOrWhiteSpace(x.BoxName))
      {
        clientOrderBox = clientOrderBoxes.FirstOrDefault(y => y.IsDefault == true);
      }
      else
      {
        var boxName = x.BoxName!.Trim();
        clientOrderBox = clientOrderBoxes.FirstOrDefault(y => !string.IsNullOrWhiteSpace(y.BoxName) && string.Equals(boxName, y.BoxName.Trim(), StringComparison.OrdinalIgnoreCase));

        if (clientOrderBox is null)
        {
          error.IsSuccessed = false;
          error.Msg.Add($"Box with name:{x.BoxName} not found.");
        }
      }
      if (clientOrderBox is not null)
      {
        var mapData = _mapper.Map<ClientOrderBoxResponseModel>(clientOrderBox);
        obj.OrderBoxs!.Add(mapData);
      }
      #endregion
      obj.Remarks = x.Remarks;
      obj.Description = x.Description;
      obj.RefNo = x.RefNo;
      errors.Add(error);
      shipments.Add(obj);
    }
    response.IsSuccessed = errors.All(x => x.IsSuccessed);
    response.UDTOrderDetail = shipments;
    //if (response.IsSuccessed)
    //{
    //}
    response.Errors = errors;
    return response;
  }
  public static CreateUploadOrderResponseModel GetCreateOrderCommand(UDTOrderDetailSimplified x)
  {
    return new CreateUploadOrderResponseModel
    {
      StoreId = 0,
      OrderTypeId = (int)EnumOrderType.Regular,
      OrderDate = DateTime.UtcNow,
      Description = x.Description,
      Remarks = x.Remarks,
      Amount = x.Amount,
      CShippingCharges = 0,
      PaymentStatusId = 0,
      Weight = 0,
      ItemValue = 0,
      OrderRequestVia = (int)EnumOrderRequestVia.Api,
      PaymentMethodId = 1,
      StationId = 0,
      Discount = 0,
      VAT = 0,
      OrderNote = new OrderNoteModel { Note = "" },
      OrderAddress = new OrderAddressResponseModel()
      {
        CustomerName = "",
        Email = "",
        Mobile1 = "",
        Mobile2 = "",
        StreetAddress = ""
      },
      OrderItems = new List<OrderItemModel>()
    };
  }

}

public class CountryFormField
{
  public bool Required { get; set; }
  public string? Label { get; set; }
  public string? Type { get; set; }
}

public class CountryMainForm
{
  public string? Country { get; set; }
  public List<string>? Keys { get; set; }
  [JsonExtensionData]
  public Dictionary<string, JToken>? Fields { get; set; }
}

//........product upload..............
public class CreateUploadProductResponseModel
{
  public string? Title { get; set; }
  public string SKU { get; set; } = null!;
  public string? ProductName { get; set; }
  public decimal? Price { get; set; }
  public decimal? PurchasePrice { get; set; }
  public string? Description { get; set; }
  public int? ProductCategoryId { get; set; }
  public string? FeatureImage { get; set; }
  public decimal? Weight { get; set; }
  public bool HaveOptions { get; set; }
  public int? QuantityAvailable { get; set; }
  public int? StoreId { get; set; }
  public int? ProductStatusId { get; set; }
  public bool? TrackInventory { get; set; }
  public List<ProductOptionUplaoadResponseModel>? ProductOptions { get; set; } = new();
  public List<ProductStockReuqestModel>? ProductStocks { get; set; } = new();

}
public class UDTProductDetailResponse
{
  public bool IsSuccessed { get; set; }
  public List<CreateUploadProductResponseModel>? UDTOrderDetail { get; set; }
  public List<UDTProductFileUploadError>? Errors { get; set; }
}
public class UDTProductFileUploadError
{
  public int Row { get; set; }
  public bool IsSuccessed { get; set; }
  public List<string> Msg { get; set; } = new List<string>();
}
//For Product.....
public class UDTproductDetailSimplified : UDTProductDetail
{
  public static UDTProductDetailResponse ConvertoProductOrderDetail(IList<UDTproductDetailSimplified> dataList, int defaultStationId, IMapper _mapper, List<ProductOptionLookup>? productOptionLookups, List<ProductCategory> productCategories, List<Store> allstores)
  {
    var response = new UDTProductDetailResponse();
    var shipments = new List<CreateUploadProductResponseModel>();
    var errors = new List<UDTProductFileUploadError>();

    var i = 0;
    foreach (var x in dataList)
    {
      i++;
      var obj = GetCreatProductCommand(x)!;
      var error = new UDTProductFileUploadError
      {
        Row = i,
        IsSuccessed = obj != null
      };
      //Fill Model here. 
      //obj!.SKU = GetProductSku();
      #region product inventory
      if (!string.IsNullOrEmpty(x.HaveOption) && x.HaveOption.Trim().ToLower() == "y")
      {
        obj!.HaveOptions = true;
        //if (!string.IsNullOrEmpty(x.Inventory))
        //{
        //  var inventorys = x.Inventory.Split(","); //it will Size_small-mediam-large,color_red-black

        //  foreach (var item in inventorys)
        //  {
        //    // now split  Size_small-mediam-large

        //    var optionNameArr = item.Split('_'); //size

        //    var optionNAme = optionNameArr.FirstOrDefault(); //optiona name - size
        //    var optionValues = optionNameArr.LastOrDefault(); // option value small-mediam-large


        //    if (!string.IsNullOrEmpty(optionNAme))
        //    {
        //      ProductOptionLookup? productOptionLookup = productOptionLookups!.FirstOrDefault(y => y.Name!.Trim().ToLower() == optionNAme.Trim().ToLower());
        //      if (productOptionLookup is not null)
        //      {
        //        if (!string.IsNullOrEmpty(optionValues))
        //        {
        //          var optionValuArr = optionValues!.Split("-");
        //          int dispalyOrder = 1;
        //          foreach (var optionValue in optionValuArr)
        //          {
        //            obj.ProductOptions!.Add(new ProductOptionUplaoadResponseModel
        //            {
        //              ProductOptionsId = productOptionLookup.ProductOptionId,
        //              OptionValue = optionValue,
        //              DisplayOrder = dispalyOrder
        //            });
        //            dispalyOrder++;
        //          }

        //        }
        //      }
        //      else
        //      {
        //        error.IsSuccessed = false;
        //        error.Msg.Add($"No Product Option found against:{optionNAme}.");
        //      }
        //    }
        //    else
        //    {
        //      error.IsSuccessed = false;
        //      error.Msg.Add("Invalid inventory data.");
        //    }
        //    

        //  }
        //  // now we have

        //}

      }
      else
      {
        obj!.HaveOptions = false;

      }
      #endregion
      #region Description  
      if (!string.IsNullOrEmpty(x.Description))
      {
        obj.Description = x.Description;
      }
      else
      {
        error.IsSuccessed = false;
        error.Msg.Add("Description is Missing.");
      }
      #endregion
      #region Store Name  
      if (!string.IsNullOrEmpty(x.StoreName))
      {
        //get all category like option lookup
        var store = allstores!.FirstOrDefault(y => y.StoreName!.Trim().ToLower() == x.StoreName.Trim().ToLower());
        if (store is not null)
        {
          obj.StoreId = store.StoreId;
        }
        else
        {
          error.IsSuccessed = false;
          error.Msg.Add($"No store Found against:{x.StoreName}.");
        }
      }
      else
      {
        error.IsSuccessed = false;
        error.Msg.Add("store Name  is Missing.");
      }
      #endregion
      #region Title
      if (!string.IsNullOrEmpty(x.Title))
      {
        obj.Title = x.Title;
      }
      else
      {
        error.IsSuccessed = false;
        error.Msg.Add("Title is Missing.");
      }
      #endregion
      #region Weight
      if (x.Weight > 0)
      {
        obj.Weight = x.Weight;
      }
      else
      {
        error.IsSuccessed = false;
        error.Msg.Add("Weight is Missing.");
      }
      #endregion 
      #region sale Price
      if (x.SalePrice > 0)
      {
        obj.Price = x.SalePrice;
      }
      else
      {
        error.IsSuccessed = false;
        error.Msg.Add("Sale price is Missing.");
      }
      #endregion
      #region Purchase Price
      if (x.PurchasePrice > 0)
      {
        obj.PurchasePrice = x.PurchasePrice;
      }
      else
      {
        error.IsSuccessed = false;
        error.Msg.Add("Purchase price is Missing.");
      }
      #endregion
      #region Category  
      if (!string.IsNullOrEmpty(x.CategoryName))
      {
        //get all category like option lookup
        var prodCategory = productCategories!.FirstOrDefault(y => y.CategoryName!.Trim().ToLower() == x.CategoryName.Trim().ToLower());
        if (prodCategory is not null)
        {
          obj.ProductCategoryId = prodCategory.ProductCategoryId;
        }
        else
        {
          error.IsSuccessed = false;
          error.Msg.Add("Category Name  is Missing.");
        }
      }

      #endregion
      #region SKU  
      if (!string.IsNullOrEmpty(x.Sku))
      {
        obj.SKU = x.Sku;
      }
      else
      {
        error.IsSuccessed = false;
        error.Msg.Add("SKU is Missing.");
      }
      #endregion
      #region Quantity available
      if (x.QtyAvailable > 0)
      {
        obj.QuantityAvailable = x.QtyAvailable;
      }
      else
      {
        error.IsSuccessed = false;
        error.Msg.Add("Quantity available is Missing.");
      }
      #endregion
      errors.Add(error);
      if (obj != null)
        shipments.Add(obj);
    }

    response.IsSuccessed = errors.All(x => x.IsSuccessed);
    response.UDTOrderDetail = shipments;
    response.Errors = errors;
    return response;
  }
  #region random sku
  // private static string GetProductSku()
  //{
  //  Random random = new Random();
  //  int randomNumber = random.Next(1000000);
  //  const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
  //  StringBuilder randomWord = new StringBuilder();

  //  for (int i = 0; i < 4; i++)
  //  {
  //    randomWord.Append(chars[random.Next(chars.Length)]);
  //  }

  //  string generatedSku = $"{randomWord.ToString()}{randomNumber}";
  //  return generatedSku;
  //}
  #endregion

  public static CreateUploadProductResponseModel GetCreatProductCommand(UDTproductDetailSimplified x)
  {
    return new CreateUploadProductResponseModel

    {

    };

  }
}
//Defined class for product...
public class UDTProductDetail
{
  public string? Title { get; set; }
  public string? Description { get; set; }
  public string? Sku { get; set; }
  public string? CategoryName { get; set; }
  public string? StoreName { get; set; }
  //public string? Inventory { get; set; }
  public int QtyAvailable { get; set; }
  public decimal Weight { get; set; }
  public decimal PurchasePrice { get; set; }
  public decimal SalePrice { get; set; }
  public string? HaveOption { get; set; } = "n";
}


