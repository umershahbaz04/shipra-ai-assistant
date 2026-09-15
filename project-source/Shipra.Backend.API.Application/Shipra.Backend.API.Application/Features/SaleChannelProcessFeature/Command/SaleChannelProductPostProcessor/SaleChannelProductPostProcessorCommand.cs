using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.SaleChannelProcessFeature.Command.SaleChannelProductPostProcessor;
public class SaleChannelProductPostProcessorCommand : IRequest<ServiceResultDTO>
{
  public string? ProductIds { get; set; }
  public string? Skus { get; set; }
  public int SaleChannelConfigId { get; set; }
  public int? SaleChannelLookupId { get; set; }
  public int? StoreId { get; set; }
}
