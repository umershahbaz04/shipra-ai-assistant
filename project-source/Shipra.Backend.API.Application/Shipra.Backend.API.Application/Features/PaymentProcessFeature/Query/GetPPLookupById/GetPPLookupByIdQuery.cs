using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.PaymentProcessFeature.Query.GetPPLookupById;
public class GetPPLookupByIdQuery : IRequest<ServiceResultDTO>
{
  public int PPLookupId { get; set; }
}
