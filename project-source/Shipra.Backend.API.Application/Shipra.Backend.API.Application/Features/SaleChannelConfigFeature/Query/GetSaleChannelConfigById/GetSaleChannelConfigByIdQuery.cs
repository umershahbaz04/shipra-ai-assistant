using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Query.GetSaleChannelConfigById;
public class GetSaleChannelConfigByIdQuery : IRequest<ServiceResultDTO>
{
  public int SaleChannelConfigId { get; set; }
}
