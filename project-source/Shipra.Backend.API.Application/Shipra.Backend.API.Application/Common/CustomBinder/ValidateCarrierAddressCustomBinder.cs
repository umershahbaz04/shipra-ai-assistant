using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.UpdateOrder;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.ValidatedOrderAddressForCarrier;

namespace Shipra.Backend.API.Application.Common.CustomBinder;
public class ValidateCarrierAddressCustomBinder : IModelBinder
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
        var clientRequest = JsonConvert.DeserializeObject<ValidateOrderAddressClientSideRequestModel>(rawRequestBody,
     new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore });

        if (clientRequest != null)
        {
          // Convert client-side request to CreateOrderCommand
          var createOrderCommand = new ValidateOrderAddressForCarrierQuery
          {
            CarrierId = clientRequest.CarrierId, 
            ActiveCarrierId = clientRequest.ActiveCarrierId,  
            // Convert OrderAddressClientSideModel to OrderAddressModel
            //OrderAddress = clientRequest.OrderAddress != null
            //        ? AddressConversionHelper.ConvertToOrderAddressValidateCarrierModel(clientRequest.OrderAddress)
            //        : null, 
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

