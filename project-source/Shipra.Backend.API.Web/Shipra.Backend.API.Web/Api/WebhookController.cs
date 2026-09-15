using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Stripe;

namespace Shipra.Backend.API.Web.Api;
[Route("api/[controller]")]
[ApiController]
public class WebhookController : ControllerBase
{

  private readonly IOrderRepository _orderRepository;
  private readonly IClientRepository _clientRepository;
  public WebhookController(IOrderRepository orderRepository, IClientRepository clientRepository)
  {
    _orderRepository = orderRepository;
    _clientRepository = clientRepository;
  }

  // This is your Stripe CLI webhook secret for testing your endpoint locally.
  //const string endpointSecret = "whsec_nr8gcVeQgoqQYiIIX46dAOvM5pWyFpBX";
  #region stripe
  string? endpointSecret;

  [HttpPost]
  public async Task<IActionResult> Index()
  {
    bool isRequestValidate = false;
    var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

    Root? root = JsonConvert.DeserializeObject<Root?>(json);
    if (root is not null)
    {
      if (root.type == EventTypes.InvoicePaid)
      {
        string jsonString = JsonConvert.SerializeObject(root.data, Formatting.Indented);
        dynamic? jsonObject = JsonConvert.DeserializeObject<dynamic>(jsonString!);
        dynamic? objectData = jsonObject!["object"];
        var invoiceId = objectData.id.Value;
        if (!string.IsNullOrEmpty(invoiceId))
        {
          Shipra.Backend.API.Core.OrderAggregate.Order order = await _orderRepository.GetOrderByStripeInvoiceId(invoiceId);
          if (order is not null)
          {
            var client = await _clientRepository.GetClientById(order.ClientId!);
            if (client is not null)
            {
              if (!string.IsNullOrEmpty(client.StripeWebhookSecret))
              {
                endpointSecret = client.StripeWebhookSecret;
                isRequestValidate = true;
              }
              else
              {
                return BadRequest("StripeWebhookSecret not found");
              }
            }
            else
            {
              return BadRequest("client Entity not found");
            }
          }
          else
          {
            return BadRequest("Order Entity not found");
          }
        }
        else
        {
          return BadRequest("Invoice Id not found");
        }
      }
    }
    try
    {
      if (isRequestValidate)
      {
        var stripeEvent = EventUtility.ConstructEvent(json,
          Request.Headers["Stripe-Signature"], endpointSecret);

        // Handle the event
        switch (stripeEvent.Type)
        {
          case EventTypes.InvoicePaid:
            var data = stripeEvent.Data.Object as Invoice;
            var invoiceId = data!.Id;
            if (!string.IsNullOrEmpty(invoiceId))
            {
              var order = await _orderRepository.GetOrderByStripeInvoiceId(invoiceId);
              if (order is not null)
              {
                order.UpdateOrderPaymentStatusViaStripeInvoicePaymentStatus((int)EnumPaymentStatus.Paid, (int)EnumPaymentMethod.PP);
                await _orderRepository.UpdateOrder(order);
              }
            }
            break;
          default:
            _ = $"Unhandled event type:, {stripeEvent.Type}";
            break;
        }
        return Ok();
      }
      else { return BadRequest("Request not valid"); }
    }
    catch (StripeException e)
    {
      return BadRequest(e.Message);
    }
  }

  #endregion

}

public class Root
{
  public string? id { get; set; }
  public string? @object { get; set; }
  public string? api_version { get; set; }
  public int created { get; set; }
  public object? data { get; set; }
  public bool livemode { get; set; }
  public int pending_webhooks { get; set; }
  public object? request { get; set; }
  public string? type { get; set; }
}

public class ObjectData
{
  public string? id { get; set; }
}
