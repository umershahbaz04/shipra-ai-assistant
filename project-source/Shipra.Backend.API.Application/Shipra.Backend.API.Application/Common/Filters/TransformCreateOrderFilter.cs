using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrder; 
namespace Shipra.Backend.API.Application.Common.Filters;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Http;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.Helper; 

public class TransformClientOrderFilter : IAsyncActionFilter
{
  public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
  {
    bool isClientRequest = false;

    // Determine if this is a client request
    if (context.HttpContext.Request.Headers.TryGetValue("X-Client-Request", out var clientRequestHeader) &&
        clientRequestHeader == "true")
    {
      isClientRequest = true; // Identified via header
    }
    else if (context.HttpContext.Request.Query.ContainsKey("isClientRequest"))
    {
      isClientRequest = true; // Identified via query parameter
    }

    // If it's a client request, read the body manually
    if (isClientRequest)
    {
      context.HttpContext.Request.EnableBuffering(); // Enable request body buffering

      using (var reader = new StreamReader(context.HttpContext.Request.Body, Encoding.UTF8, leaveOpen: true))
      {
        var rawRequestBody = await reader.ReadToEndAsync();
        context.HttpContext.Request.Body.Position = 0; // Reset stream position

        if (!string.IsNullOrWhiteSpace(rawRequestBody))
        {
          try
          {
            // Deserialize client-side model
            var clientRequest = JsonSerializer.Deserialize<CreateOrderClientSideRequestModel>(
                rawRequestBody,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (clientRequest != null)
            {
              // Transform to CreateOrderCommand
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

                  OrderAddress = item.OrderAddress != null
                        ? new OrderAddressModel
                        {
                          AreaId = ExtractAreaId(item.OrderAddress.CityId)
                        }
                        : null,

                  OrderItems = item.OrderItems,
                  OrderTaxes = item.OrderTaxes
                }).ToList(),
                IsSaleChannelOrder = clientRequest.IsSaleChannelOrder,
                OrderDraftId = clientRequest.OrderDraftId,
              };

              // Replace the action argument
              context.ActionArguments["request"] = createOrderCommand;
            }
          }
          catch (JsonException ex)
          {
            context.Result = new BadRequestObjectResult(new { error = "Invalid JSON format", details = ex.Message });
            return;
          }
        }
      }
    }

    // Proceed to the next filter or the action method
    await next();
  }

  private string GetAddressJson(OrderAddressClientSideModel? orderAddress)
  {
    var jsonList = new List<Dictionary<string, object>>();

    CivilEntityHelper.AddToJsonList(jsonList, "CityId", orderAddress?.CityId);
    CivilEntityHelper.AddToJsonList(jsonList, "AreaId", orderAddress?.AreaId);
    CivilEntityHelper.AddToJsonList(jsonList, "ProvinceId", orderAddress?.ProvinceId);
    CivilEntityHelper.AddToJsonList(jsonList, "StateId", orderAddress?.StateId);
    CivilEntityHelper.AddToJsonList(jsonList, "PinCodeId", orderAddress?.PinCodeId);

    // Do something with jsonList, like returning count or processing further
    return Newtonsoft.Json.JsonConvert.SerializeObject(jsonList);
  }

  private int ExtractAreaId(string? cityId)
  {
    if (!string.IsNullOrEmpty(cityId) && cityId.Contains("_"))
    {
      var parts = cityId.Split('_');
      if (int.TryParse(parts[0], out int extractedAreaId))
      {
        return extractedAreaId;
      }
    }
    return 0; // Return 0 if extraction fails
  }
}
