using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.SaleChannelProcessFeature.Command.SaleChannelOrderPostProcessor;
public class SaleChannelOrderPostProcessorCommand : IRequest<ServiceResultDTO>
{
  public string? OrderIds { get; set; }
  public int? SaleChannelConfigId { get; set; }
  public int? SaleChannelLookupId { get; set; }
  public int? StoreId { get; set; }
  public int? OrderTypeId { get; set; }
}
