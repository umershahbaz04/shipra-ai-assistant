using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Stripe;

namespace Shipra.Backend.API.Application.Features.PaymentProcessFeature.Query.GetStripeWebhook;

public class GetStripeWebhookCommandHandler : RequestHandlerBase<GetStripeWebhookCommand, ServiceResultDTO>
{
  private readonly IOrderRepository _orderRepository;
  private readonly IClientRepository _clientRepository;

  public GetStripeWebhookCommandHandler(IOrderRepository orderRepository, IClientRepository clientRepository, IServiceProvider serviceProvider, ILogger<GetStripeWebhookCommandHandler> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;
    _clientRepository = clientRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetStripeWebhookCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      string? endpointSecret;
      Root? root = JsonConvert.DeserializeObject<Root?>(request.JsonData!);
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

                  //var stripeEvent = EventUtility.ConstructEvent(jsonString, Request.Headers["Stripe-Signature"], endpointSecret);

                  //// Handle the event
                  //switch (stripeEvent.Type)
                  //{
                  //  case Events.InvoicePaid:
                  //    var data = stripeEvent.Data.Object as Invoice;
                  //    var invoiceId = data!.Id;
                  //    if (!string.IsNullOrEmpty(invoiceId))
                  //    {
                  //      var order = await _orderRepository.GetOrderByStripeInvoiceId(invoiceId);
                  //      if (order is not null)
                  //      {
                  //        order.UpdateOrderPaymentStatusViaStripeInvoicePaymentStatus((int)EnumPaymentStatus.Paid, (int)EnumPaymentMethod.PP);
                  //        await _orderRepository.UpdateOrder(order);
                  //      }
                  //    }
                  //    break;
                  //  default:
                  //    _ = $"Unhandled event type:, {stripeEvent.Type}";
                  //    break;
                  //}
                  //return Ok();
                }
                else
                {
                  throw new EntityNotFoundException("Mcconfig", "Integration Value");
                }
              }
              else
              {
                throw new EntityNotFoundException("Mcconfig", "Integration Value");
              }
            }
            else
            {
              throw new EntityNotFoundException("Mcconfig", "Integration Value");
            }
          }
          else
          {
            throw new EntityNotFoundException("Mcconfig", "Integration Value");
          }
        }
      }

      //var result = await _carrierSharedRepository.CreateWebHook(request.CarrierId, _currentUser.Id, mcconfig.Value!);

      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
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
