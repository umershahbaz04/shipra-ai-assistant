
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Http;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrder;
using Newtonsoft.Json;
using Shipra.Backend.API.Core.Helper;

namespace Shipra.Backend.API.Application.Common.CustomBinder;
public class CreateOrderClientSideBinder : IModelBinder
{
  public async Task BindModelAsync(ModelBindingContext bindingContext)
  {
    if (bindingContext == null)
    {
      throw new ArgumentNullException(nameof(bindingContext));
    }

    // Read the request body as a raw JSON string
    var request = bindingContext.HttpContext.Request;
    request.EnableBuffering(); // Allows the request stream to be read multiple times

    using (var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true))
    {
      var rawRequestBody = await reader.ReadToEndAsync();
      request.Body.Position = 0; // Reset stream position so it can be read again later

      if (string.IsNullOrWhiteSpace(rawRequestBody))
      {
        return;
      }

      try
      {
        // Deserialize the client-side model first
        var clientRequest = JsonConvert.DeserializeObject<CreateOrderClientSideRequestModel>(rawRequestBody,
     new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore });

        if (clientRequest != null)
        {
          // Convert client-side request to CreateOrderCommand
          var createOrderCommand = new CreateOrderCommand
          {
            orderList = clientRequest.orderList?.Select(item => new CreateOrderRequestModel
            {
              StoreId = item.StoreId,
              OrderTypeId = item.OrderTypeId,
              OrderDate = item.OrderDate,
              Description = item.Description,
              Remarks = item.Remarks,
              Amount = item.Amount,
              CShippingCharges = item.CShippingCharges,
              PaymentStatusId = item.PaymentStatusId,
              Weight = item.Weight,
              ItemValue = item.ItemValue,
              OrderRequestVia = item.OrderRequestVia,
              PaymentMethodId = item.PaymentMethodId,
              StationId = item.StationId,
              Discount = item.Discount,
              VAT = item.VAT,
              RefNo = item.RefNo,
              SaleChannelConfigId = item.SaleChannelConfigId,
              SaleChannelLookupId = item.SaleChannelLookupId,
              OrderDeliveryTypeId = item.OrderDeliveryTypeId,
              OrderNote = item.OrderNote,
              OrderBoxs = item.OrderBoxs,  
              // Convert OrderAddressClientSideModel to OrderAddressModel
              OrderAddress = item.OrderAddress != null
                    ? AddressConversionHelper.ConvertToOrderAddressModel(item.OrderAddress)
                    : null,
              settingConfig = item.settingConfig,
              OrderItems = item.OrderItems,
              OrderTaxes = item.OrderTaxes,
              CarrierData = item.CarrierData
            }).ToList(),
            
            IsSaleChannelOrder = clientRequest.IsSaleChannelOrder,
            OrderDraftId = clientRequest.OrderDraftId,
            IsAssigCarrier = clientRequest.IsAssigCarrier,
            WithThirdPartyResponse = clientRequest.WithThirdPartyResponse
          };

          // Set the result as the transformed model
          bindingContext.Result = ModelBindingResult.Success(createOrderCommand);
        }
      }
      catch (System.Text.Json.JsonException ex)
      {
        bindingContext.ModelState.AddModelError("JSON", "Invalid JSON format: " + ex.Message);
      }
    }
  }
}
