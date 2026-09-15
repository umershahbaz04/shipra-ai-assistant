using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.SaleChannelProcessFeature.Query.SaleChannelProductPreProcessor;
public class SaleChannelProductPreProcessorCommand : IRequest<ServiceResultDTO>
{
  public int StoreId { get; set; }
  public int SaleChannelConfigId { get; set; }
  public DateTime CreatedFrom { get; set; }
  public DateTime CreatedTo { get; set; }
}
