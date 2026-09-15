using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.PaymentProcessFeature.Query.GetStripeWebhook;
public class GetStripeWebhookCommand : IRequest<ServiceResultDTO>
{
  public string? JsonData { get; set; }
}
