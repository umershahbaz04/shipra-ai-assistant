using DocumentFormat.OpenXml.Bibliography;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Application.DTOs.SaleChannelUseCase.WooCommerece;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrder;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.SaleChannelConfigAggregate;
using Shipra.Backend.API.Core.StoresAggregate;

namespace Shipra.Backend.API.Application.DTOs.SaleChannelUseCase.WooCommereceOrder;
public class WoocommerceOrderDetailSimplified
{
  public static WoocommerceOrderDetailResponse ConverWooCommercetoShipraOrderDetail(List<OrderWooCommerceModal>? dataList, List<Country>? countries, List<Core.CountryAggregate.City> cities, List<Province> provinces, List<State> states, List<Area> areas, List<PinCode> pinCodes, Store? oStore, int defaultStationId, SaleChannelConfig oSaleChannelConfig, int? saleChannelLookupId, int? orderTypeId, IEnumerable<dynamic>? castedList = null)
  {
    var response = new WoocommerceOrderDetailResponse();
    var shipments = new List<CreateUploadOrderResponseModel>();
    var errors = new List<WoocommerceOrderError>();

    var i = 0;
    foreach (var x in dataList!)
    {
      i++;
      var obj = GetCreateOrderCommand();
      var error = new WoocommerceOrderError();

      #region Index 
      error.Row = i;
      error.IsSuccessed = true;
      #endregion

      obj.StationId = defaultStationId;
      obj.StoreId = oStore!.StoreId;
      obj.Weight = 0;
      obj.VAT = x.TotalTax;
      obj.ItemValue = x.Total;
      obj.Discount = x.DiscountTotal;
      obj.SaleChannelConfigId = oSaleChannelConfig.SaleChannelConfigId;
      obj.SaleChannelLookupId = saleChannelLookupId;
      obj.OrderDate = x.DateCreated!;
      #region Amount
      if (x.Total > 0)
      {
        obj.Amount = x.Total;
      }
      else
      {
        obj.Amount = 0;
      }
      #endregion

      #region Payment Method
      if (!string.IsNullOrEmpty(x.PaymentMethod))
      {
        if (x.PaymentMethod == "other")
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
      var wooCommerceOrderAddress = x.Shipping is not null ? x.Shipping : x.Billing;
      if (wooCommerceOrderAddress is null)
      {
        wooCommerceOrderAddress = null;
      }

      #region Customer Name
      if (!string.IsNullOrEmpty(wooCommerceOrderAddress!.first_name))
      {
        obj.OrderAddress!.CustomerName = wooCommerceOrderAddress.first_name + (wooCommerceOrderAddress.last_name != null ? " " + wooCommerceOrderAddress.last_name : "");
      }
      else
      {
        error.IsSuccessed = false;
        error.Msg.Add("Customer Name is Missing.");
      }
      #endregion

      #region Address

      //#region Latitude  
      //if (shopifyOrderAddress.Latitude != null && shopifyOrderAddress.Latitude > 0 && shopifyOrderAddress.Longitude != null && shopifyOrderAddress.Longitude > 0)
      //{
      //  var decimalPlaces = 6;
      //  if (shopifyOrderAddress.Latitude != null && shopifyOrderAddress.Latitude > 0)
      //  {
      //    var latitude = shopifyOrderAddress.Latitude!.ToString();
      //    if (Utility.ValidateLatitude(latitude!))
      //    {
      //      var truncatedValue = Math.Truncate(shopifyOrderAddress.Latitude.GetValueOrDefault() * (decimal)Math.Pow(10, decimalPlaces)) / (decimal)Math.Pow(10, decimalPlaces);

      //      obj.OrderAddress!.Latitude = truncatedValue;
      //    }
      //    else
      //    {
      //      error.IsSuccessed = false;
      //      error.Msg.Add("Latitude is invalid.");
      //    }
      //  }

      //  if (shopifyOrderAddress.Longitude != null && shopifyOrderAddress.Longitude > 0)
      //  {
      //    var longitude = shopifyOrderAddress.Longitude!.ToString();
      //    if (Utility.ValidateLongitude(longitude!))
      //    {
      //      var truncatedValue = Math.Truncate(shopifyOrderAddress.Longitude.GetValueOrDefault() * (decimal)Math.Pow(10, decimalPlaces)) / (decimal)Math.Pow(10, decimalPlaces);

      //      obj.OrderAddress!.Longitude = truncatedValue;
      //    }
      //    else
      //    {
      //      error.IsSuccessed = false;
      //      error.Msg.Add("Longitude is invalid.");
      //    }
      //  }
      //}
      //#endregion

      Country? country = null;
      #region country,city,region

      if (!string.IsNullOrEmpty(wooCommerceOrderAddress.country))
      {
        country = countries!.Where(y => y.MapCountryCode?.Trim().ToLower() == wooCommerceOrderAddress.country.Trim().ToLower()).FirstOrDefault();

        if (country != null)
        {
          obj.OrderAddress!.Country = country.CountryId;
        }
        else
        {
          error.IsSuccessed = false;
          error.Msg.Add("Invaild Country Code.");
        }

        if (!string.IsNullOrEmpty(wooCommerceOrderAddress.state))
        {
          var oProvince = new Province();
          var oState = new State();
          if (country != null)
          {
            oProvince = provinces.Where(y => y.Name!.Trim().ToLower() == wooCommerceOrderAddress.state.Trim().ToLower() && country.CountryId == y.CountryId).FirstOrDefault();
            if (oProvince == null)
            {
              oState = states.Where(y => y.Name!.Trim().ToLower() == wooCommerceOrderAddress.state.Trim().ToLower() && country.CountryId == y.CountryId).FirstOrDefault();
            }
          }

          if (oProvince != null)
          {
            obj.OrderAddress!.Province = oProvince.ProvinceId;

            if (!string.IsNullOrEmpty(wooCommerceOrderAddress.city))
            {
              var oCity = cities.Where(y => y.Code!.Trim().ToLower() == wooCommerceOrderAddress.city.Trim().ToLower() && oProvince.ProvinceId == y.ProvinceId).FirstOrDefault();
              if (oCity != null)
              {
                obj.OrderAddress!.City = oCity.CityId;
              }
            }
          }
          else if (oState != null)
          {
            obj.OrderAddress!.State = oState.StateId;

            if (!string.IsNullOrEmpty(wooCommerceOrderAddress.city))
            {
              var oCity = cities.Where(y => y.Code!.Trim().ToLower() == wooCommerceOrderAddress.city.Trim().ToLower() && oState.StateId == y.StateId).FirstOrDefault();
              if (oCity != null)
              {
                obj.OrderAddress!.City = oCity.CityId;
              }
            }
          }
          else if (obj.OrderAddress!.City == null)
          {
            var oCity = cities.FirstOrDefault(y => y.Name!.Trim().ToLower() == wooCommerceOrderAddress!.city!.Trim().ToLower() && country!.CountryId == y.CountryId);
            if (oCity != null)
            {
              obj.OrderAddress!.City = oCity.CityId;
            }
            else
            {
              error.IsSuccessed = false;
              error.Msg.Add("Invaild City.");
            }
          }
        }
        else
        {
          error.IsSuccessed = false;
          error.Msg.Add("Region is Missing.");
        }
      }

      #endregion


      #region MobileNumber

      #region mobile 1
      if (!string.IsNullOrEmpty(wooCommerceOrderAddress.phone))
      {
        if (wooCommerceOrderAddress.phone.Length <= 20)
        {
          if (country != null)
          {
            var res = Utility.ValidateMobileAndGetNumber(wooCommerceOrderAddress.phone, country!.MapCountryCode!.ToUpper());
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
            obj.OrderAddress!.Mobile1 = wooCommerceOrderAddress.phone;
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

      obj.OrderAddress!.Mobile2 = wooCommerceOrderAddress.phone;

      #endregion

      #endregion

      #region StreetAddress

      var completeAddress = wooCommerceOrderAddress.address_1 + "," + wooCommerceOrderAddress.state + "," + wooCommerceOrderAddress.country;
      obj.OrderAddress!.StreetAddress = completeAddress;

      #endregion

      #endregion

      #region product

      if (orderTypeId == (int)EnumOrderType.Regular)
      {
        foreach (var item in x.LineItems!)
        {
          obj.OrderItems!.Add(new OrderItemModel
          {
            ProductId = "",
            ProductVariantId = 0,
            Price = item.Price,
            Description = $"Name :{item.Name} - QTY: {item.Quantity}",
            Remarks = string.Empty,
            Quantity = item.Quantity,
            Discount = 0
          });
        }
      }
      else if (orderTypeId == (int)EnumOrderType.FullFilable)
      {
        var fileProductWithErr = new List<string>();
        var validatedFileProduct = new List<string>();
        foreach (var item in x.LineItems!)
        {
          //filter product against station and sku
          var product = castedList!.FirstOrDefault(product => product.SaleChannelVariantId == item.Id);

          if (product != null)
          {
            obj.OrderItems!.Add(new OrderItemModel
            {
              ProductId = product.ProductId,
              ProductVariantId = product.ProductVariantId,
              SaleChannelVariantId = item.VariationId,
              Price = item.Price,
              Description = $"Name :{item.Name} - QTY: {item.Quantity}",
              Remarks = string.Empty,
              Quantity = item.Quantity,
              Discount = 0
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

      obj.Remarks = x.CustomerNote;
      obj.Description = OrderCommon.GetDescriptionForWooCommerceOrderItems(x.LineItems!);
      obj.RefNo = x.Number != null ? x.Number : "";
      errors.Add(error);
      shipments.Add(obj);
    }
    response.IsSuccessed = errors.All(x => x.IsSuccessed);
    response.WoocommerceOrderDetail = shipments;
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
