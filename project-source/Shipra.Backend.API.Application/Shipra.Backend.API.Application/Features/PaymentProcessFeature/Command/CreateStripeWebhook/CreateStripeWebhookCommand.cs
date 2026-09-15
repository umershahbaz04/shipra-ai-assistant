using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.PaymentProcessFeature.Command.CreateStripeWebhook;
public class CreateStripeWebhookCommand : IRequest<ServiceResultDTO>
{
}
