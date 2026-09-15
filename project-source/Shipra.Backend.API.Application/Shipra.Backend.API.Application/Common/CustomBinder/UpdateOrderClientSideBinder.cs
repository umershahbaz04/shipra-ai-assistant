using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrder;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.UpdateOrder;
using Shipra.Backend.API.Core.Helper;

namespace Shipra.Backend.API.Application.Common.CustomBinder;
public class UpdateOrderClientSideBinder : IModelBinder
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
        var clientRequest = JsonConvert.DeserializeObject<UpdateOrderClientSideRequestModel>(rawRequestBody,
     new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore });

        if (clientRequest != null)
        {
          // Convert client-side request to CreateOrderCommand
          var createOrderCommand = new UpdateOrderCommand
          {
            OrderId = clientRequest.OrderId,
            StoreId = clientRequest.StoreId,
            OrderTypeId = clientRequest.OrderTypeId,
            OrderDate = clientRequest.OrderDate,
            Description = clientRequest.Description,
            Remarks = clientRequest.Remarks,
            Amount = clientRequest.Amount,
            CShippingCharges = clientRequest.CShippingCharges,
            PaymentStatusId = clientRequest.PaymentStatusId,
            Weight = clientRequest.Weight,
            ItemValue = clientRequest.ItemValue,
            OrderRequestVia = clientRequest.OrderRequestVia,
            PaymentMethodId = clientRequest.PaymentMethodId,
            StationId = clientRequest.StationId,
            Discount = clientRequest.Discount.GetValueOrDefault(),
            VAT = clientRequest.VAT.GetValueOrDefault(),
            RefNo = clientRequest.RefNo,
            OrderDraftId = clientRequest.OrderDraftId, 
            SaleChannelConfigId = clientRequest.SaleChannelConfigId, 
            SaleChannelLookupId = clientRequest.SaleChannelLookupId, 
            OrderNote = clientRequest.OrderNote,
            OrderBoxs = clientRequest.OrderBoxs,
            settingConfig = clientRequest.settingConfig,

            // Convert OrderAddressClientSideModel to OrderAddressModel
            OrderAddress = clientRequest.OrderAddress != null
                    ? AddressConversionHelper.ConvertToOrderAddressModel(clientRequest.OrderAddress)
                    : null,

            OrderItems = clientRequest.OrderItems,
            OrderTaxes = clientRequest.OrderTaxes

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
