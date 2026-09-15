using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.AccountFeature.Commands.CreateCarrierPaymentSettlement;
public class CreateCarrierPaymentSettlementCommand : IRequest<ServiceResultDTO>
{
  public List<CreateCarrierPaymentSettlementRequestModel>? list { get; set; }
  public int CarrierId { get; set; }
}
