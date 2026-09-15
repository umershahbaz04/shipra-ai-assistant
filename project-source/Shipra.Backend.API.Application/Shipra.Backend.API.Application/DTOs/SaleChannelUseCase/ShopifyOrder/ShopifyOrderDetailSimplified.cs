using Newtonsoft.Json;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Application.DTOs.OrderUseCase.HelperOrder.UDTOrder;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrder;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.SaleChannelConfigAggregate;
using Shipra.Backend.API.Core.StoresAggregate;
using Utility = Shipra.Backend.API.Application.Helpers.Utility;

namespace Shipra.Backend.API.Application.DTOs.SaleChannelUseCase.ShopifyOrder;
public class ShopifyOrderDetailSimplified
{
  public static ShopifyOrderDetailResponse ConverShopifytoShipraOrderDetail(List<ShopifySharp.Order>? dataList, List<Country>? countries, List<City> cities, List<Province> provinces, List<State> states, List<Area> areas, List<PinCode> pinCodes, Store? oStore, int defaultStationId, SaleChannelConfig oSaleChannelConfig, int? saleChannelLookupId, int? orderTypeId, IEnumerable<dynamic>? castedList = null)
  {
    var response = new ShopifyOrderDetailResponse();
    var shipments = new List<CreateUploadOrderResponseModel>();
    var errors = new List<ShopifyOrderError>();

    var i = 0;
    foreach (var x in dataList!)
    {
      i++;
      var obj = GetCreateOrderCommand();
      var error = new ShopifyOrderError();

      #region Index 
      error.Row = i;
      error.IsSuccessed = true;
      #endregion

      obj.StationId = defaultStationId;
      obj.StoreId = oStore!.StoreId;
      obj.Weight = x.TotalWeight != null ? (x.TotalWeight / 1000) : 0;
      obj.VAT = x.TotalTax;
      obj.ItemValue = x.TotalPrice;
      obj.Discount = x.TotalDiscounts;
      obj.SaleChannelConfigId = oSaleChannelConfig.SaleChannelConfigId;
      obj.ChannelId = oSaleChannelConfig.SaleChannelConfigId;
      obj.SaleChannelLookupId = saleChannelLookupId;
      obj.OrderDate = x.CreatedAt!.Value.DateTime;
      obj.OrderTypeId = orderTypeId;
      obj.SCOrderNo = x.OrderNumber.ToString();
      obj.SCOrderId = x.Id.ToString();
      #region Amount
      if (x.TotalPrice > 0)
      {
        obj.Amount = x.TotalPrice;
      }
      else
      {
        obj.Amount = 0;
      }
      #endregion

      #region Payment Method
      if (!string.IsNullOrEmpty(x.FinancialStatus))
      {
        if (x.FinancialStatus == "pending")
        {
          obj.PaymentStatusId = (int)EnumPaymentStatus.Unpaid;
          obj.PaymentMethodId = (int)EnumPaymentMethod.COD;
        }
        else
        {
          //if no payment method define
          obj.PaymentStatusId = (int)EnumPaymentStatus.Paid;
          obj.PaymentMethodId = (int)EnumPaymentMethod.PP;
        }
      }
      else
      {
        error.IsSuccessed = false;
        error.Msg.Add("Payment Method is Missing.");
      }
      #endregion 
      var shopifyOrderAddress = x.ShippingAddress is not null ? x.ShippingAddress : x.BillingAddress;
      if (shopifyOrderAddress is null)
      {
        shopifyOrderAddress = x.Customer!.DefaultAddress;
      }

      #region Customer Name
      if (!string.IsNullOrEmpty(shopifyOrderAddress!.Name))
      {
        obj.OrderAddress!.CustomerName = shopifyOrderAddress.Name;
      }
      else
      {
        error.IsSuccessed = false;
        error.Msg.Add("Customer Name is Missing.");
      }
      #endregion

      #region Address

      #region Latitude  
      if (shopifyOrderAddress.Latitude != null && shopifyOrderAddress.Latitude > 0 && shopifyOrderAddress.Longitude != null && shopifyOrderAddress.Longitude > 0)
      {
        var decimalPlaces = 6;
        if (shopifyOrderAddress.Latitude != null && shopifyOrderAddress.Latitude > 0)
        {
          var latitude = shopifyOrderAddress.Latitude!.ToString();
          if (Utility.ValidateLatitude(latitude!))
          {
            var truncatedValue = Math.Truncate(shopifyOrderAddress.Latitude.GetValueOrDefault() * (decimal)Math.Pow(10, decimalPlaces)) / (decimal)Math.Pow(10, decimalPlaces);

            obj.OrderAddress!.Latitude = truncatedValue;
          }
          else
          {
            error.IsSuccessed = false;
            error.Msg.Add("Latitude is invalid.");
          }
        }

        if (shopifyOrderAddress.Longitude != null && shopifyOrderAddress.Longitude > 0)
        {
          var longitude = shopifyOrderAddress.Longitude!.ToString();
          if (Utility.ValidateLongitude(longitude!))
          {
            var truncatedValue = Math.Truncate(shopifyOrderAddress.Longitude.GetValueOrDefault() * (decimal)Math.Pow(10, decimalPlaces)) / (decimal)Math.Pow(10, decimalPlaces);

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
      #region country,city,region

      if (!string.IsNullOrEmpty(shopifyOrderAddress.CountryCode))
      {
        country = countries!.Where(y => y.MapCountryCode?.Trim().ToLower() == shopifyOrderAddress.CountryCode.Trim().ToLower()).FirstOrDefault();

        if (country != null)
        {
          obj.OrderAddress!.Country = country.CountryId;
        }
        else
        {
          error.IsSuccessed = false;
          error.Msg.Add("Invaild Country Code.");
        }
        #region country,city,state etc

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
                if (!string.IsNullOrEmpty(shopifyOrderAddress.City))
                {
                  var city = cities.FirstOrDefault(y => y.Name!.Trim().ToLower() == shopifyOrderAddress.City.Trim().ToLower() && country.CountryId == y.CountryId);

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
                if (field!.Required && string.IsNullOrEmpty(shopifyOrderAddress.City))
                {
                  error.IsSuccessed = false;
                  error.Msg.Add("City is Missing.");
                }
              }
              #endregion
              #region province 
              if (key == "province")
              {
                if (!string.IsNullOrEmpty(shopifyOrderAddress.Province))
                {
                  var province = provinces.FirstOrDefault(y => y.Name!.Trim().ToLower() == shopifyOrderAddress.Province.Trim().ToLower() && country.CountryId == y.CountryId);
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
                if (field.Required && string.IsNullOrEmpty(shopifyOrderAddress.Province))
                {
                  error.IsSuccessed = false;
                  error.Msg.Add("Province is Missing.");
                }
              }
              #endregion
              #region state 
              if (key == "state")
              {
                if (!string.IsNullOrEmpty(shopifyOrderAddress.Province))
                {
                  var state = states.FirstOrDefault(y => y.Name!.Trim().ToLower() == shopifyOrderAddress.Province.Trim().ToLower() && country.CountryId == y.CountryId);
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
                if (field.Required && string.IsNullOrEmpty(shopifyOrderAddress.Province))
                {
                  error.IsSuccessed = false;
                  error.Msg.Add("State is Missing.");
                }
              }
              #endregion

              //#region area
              //else if (key == "area")
              //{
              //  var area = areas.Where(y => y.Code!.Trim().ToLower() == x.AreaCode!.Trim().ToLower()).FirstOrDefault();
              //  if (area != null)
              //  {
              //    obj.OrderAddress!.Area = area.AreaId;
              //  }

              //  if (field.Required && string.IsNullOrEmpty(x.AreaCode))
              //  {
              //    error.IsSuccessed = false;
              //    error.Msg.Add("Area is Missing.");
              //  }
              //}
              //#endregion
              //#region pinCode
              //else if (key == "pinCode")
              //{
              //  var pincode = pinCodes.Where(y => y.PinCodeValue!.Trim().ToLower() == x.PinCode!.Trim().ToLower()).FirstOrDefault();
              //  if (pincode != null)
              //  {
              //    obj.OrderAddress!.PinCode = pincode.PinCodeId;
              //  }

              //  if (field.Required && string.IsNullOrEmpty(x.PinCode))
              //  {
              //    error.IsSuccessed = false;
              //    error.Msg.Add("PinCode is Missing.");
              //  }
              //}
              //#endregion
            }
          }

          obj.OrderAddress!.Country = country.CountryId;
        }
        #endregion
        //if (!string.IsNullOrEmpty(shopifyOrderAddress.Province))
        //{
        //  var oProvince = new Province();
        //  if (country != null)
        //  {
        //    oProvince = provinces.Where(y => y.Name!.Trim().ToLower() == shopifyOrderAddress.Province.Trim().ToLower() && country.CountryId == y.CountryId).FirstOrDefault();
        //  }

        //  if (oProvince != null)
        //  {
        //    obj.OrderAddress!.Province = oProvince.ProvinceId;

        //    if (!string.IsNullOrEmpty(shopifyOrderAddress.City))
        //    {
        //      var city = cities.Where(y => y.Code!.Trim().ToLower() == shopifyOrderAddress.City.Trim().ToLower() && oProvince.ProvinceId == y.ProvinceId).FirstOrDefault();
        //      if (city != null)
        //      {
        //        obj.OrderAddress!.City = city.CityId;
        //      }
        //    }
        //  }
        //  else
        //  {
        //    error.IsSuccessed = false;
        //    error.Msg.Add("Invaild Region.");
        //  }
        //}
        //else
        //{
        //  error.IsSuccessed = false;
        //  error.Msg.Add("Region is Missing.");
        //}
      }

      #endregion

      #region MobileNumber

      #region mobile 1
     if (!string.IsNullOrEmpty(shopifyOrderAddress.Phone))
      {
        if (shopifyOrderAddress.Phone.Length <= 20)
        {
          if (country != null)
          {
            var res = Utility.ValidateMobileAndGetNumber(shopifyOrderAddress.Phone, country!.MapCountryCode!.ToUpper());
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
            obj.OrderAddress!.Mobile1 = shopifyOrderAddress.Phone;
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

      obj.OrderAddress!.Mobile2 = shopifyOrderAddress.Phone;

      #endregion

      #endregion

      #region StreetAddress

      var streetAddress = x.Customer != null && x.Customer.DefaultAddress != null ? x.Customer.DefaultAddress.Address1 : "";

      var completeAddress = x.Customer != null && x.Customer.DefaultAddress != null ? x.Customer.DefaultAddress.Address1 + "," + (!string.IsNullOrEmpty(x.Customer.DefaultAddress.Province) ? x.Customer.DefaultAddress.Province + "," : "") + x.Customer.DefaultAddress.City + "," + x.Customer.DefaultAddress.Country : "";

      if (!string.IsNullOrEmpty(completeAddress))
      {
        obj.OrderAddress!.StreetAddress = streetAddress;
        obj.OrderAddress!.CustomerFullAddress = completeAddress;
      }
      else
      {
        error.IsSuccessed = false;
        error.Msg.Add("Invalid address.");
      }
      #endregion

      #endregion

      #region product

      if (orderTypeId == (int)EnumOrderType.Regular)
      {
        foreach (var item in x.LineItems)
        {
          obj.OrderItems!.Add(new OrderItemModel
          {
            ProductId = "",
            ProductVariantId = 0,
            Price = item.Price,
            Description = $"Name :{item.Name} - QTY: {item.Quantity}",
            Remarks = string.Empty,
            Quantity = item.Quantity,
            Discount = item.TotalDiscount
          });
        }
      }
      else if (orderTypeId == (int)EnumOrderType.FullFilable)
      {
        var fileProductWithErr = new List<string>();
        var validatedFileProduct = new List<string>();
        foreach (var item in x.LineItems)
        {
          //filter product against station and sku
          var product = castedList!.FirstOrDefault(product => product.SaleChannelVariantId == item.VariantId);

          if (product != null)
          {
            obj.OrderItems!.Add(new OrderItemModel
            {
              ProductId = product.ProductId,
              ProductVariantId = product.ProductVariantId,
              SaleChannelVariantId = item.VariantId,
              Price = item.Price,
              Description = $"Name :{item.Name} - QTY: {item.Quantity}",
              Remarks = string.Empty,
              Quantity = item.Quantity,
              Discount = item.TotalDiscount
            });
            validatedFileProduct.Add($"Name :{item.Name} - QTY: {item.Quantity}");
          }
          else
          {
            fileProductWithErr.Add($"Name :{item.Name} - QTY: {item.Quantity}");
          }
        }
      }
      else
      {
        error.IsSuccessed = false;
        error.Msg.Add("Products is Missing.");
      }

      #endregion

      obj.Remarks = x.Note;
      obj.Description = OrderCommon.GetDescriptionForShopifyOrderItems(x.LineItems.ToList());
      obj.RefNo = x.OrderNumber != null ? x.OrderNumber.ToString() : "";
      errors.Add(error);
      shipments.Add(obj);
    }
    response.IsSuccessed = errors.All(x => x.IsSuccessed);
    response.ShopifyOrderDetail = shipments;
    //if (response.IsSuccessed)
    //{
    //}
    response.Errors = errors;
    return response;
  }

  public static CreateUploadOrderResponseModel GetCreateOrderCommand()
  {
    return new CreateUploadOrderResponseModel
    {
      StoreId = 0,
      OrderTypeId = (int)EnumOrderType.Regular,
      OrderDate = DateTime.UtcNow,
      Description = "",
      Remarks = string.Empty,
      Amount = 0,
      CShippingCharges = 0,
      PaymentStatusId = 0,
      Weight = 0,
      ItemValue = 0,
      OrderRequestVia = (int)EnumOrderRequestVia.Shopify,
      PaymentMethodId = (int)EnumPaymentMethod.COD,
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

