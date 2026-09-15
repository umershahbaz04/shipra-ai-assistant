using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.SaleChannelProcessFeature.Command.SaleChannelInventorySyncProcessor;
public class SaleChannelInventorySyncProcessorCommand : IRequest<ServiceResultDTO>
{
  public int? SaleChannelLookupId { get; set; }
  public int? SaleChannelConfigId { get; set; } = 0;
  public bool? ShipraTowardsSaleChannel { get; set; }
  public bool? SaleChannelTowardsShipra { get; set; }
  public string? ProductStockSkus { get; set; }
}
