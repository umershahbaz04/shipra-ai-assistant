using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Query.GetSaleChannelLookupById;
public class GetSaleChannelLookupByIdQuery : IRequest<ServiceResultDTO>
{
  public int SaleChannelLookupId { get; set; }
}
