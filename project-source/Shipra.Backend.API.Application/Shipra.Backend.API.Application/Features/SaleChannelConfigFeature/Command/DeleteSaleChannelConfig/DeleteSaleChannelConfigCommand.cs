using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Command.DeleteSaleChannelConfig;
public class DeleteSaleChannelConfigCommand : IRequest<ServiceResultDTO>
{
  public int SaleChannelConfigId { get; set; }
}
