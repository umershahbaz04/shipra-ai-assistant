using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.PaymentProcessFeature.Command.ActivatePaymentProcess;
public class ActivatePaymentProcessCommand : IRequest<ServiceResultDTO>
{
  public Dictionary<string, string>? InputParameters { get; set; }
  public bool? IsDefault { get; set; }
  public bool? IsActive { get; set; }
}
