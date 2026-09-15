using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.UpdateCustomerEmailAddress;
public class UpdateCustomerEmailAddressCommand : IRequest<ServiceResultDTO>
{
  public long? OrderAddressId { get; set; }
  public string? OrderNo { get; set; }
  public string? Email { get; set; }
  public string? CustomerName { get; set; }
  public string? Mobile1 { get; set; }
  public string? Mobile2 { get; set; }
}
