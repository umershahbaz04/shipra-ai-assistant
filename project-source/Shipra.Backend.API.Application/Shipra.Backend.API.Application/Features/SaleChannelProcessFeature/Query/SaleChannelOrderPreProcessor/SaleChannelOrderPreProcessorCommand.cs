using MediatR;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Enum;

namespace Shipra.Backend.API.Application.Features.SaleChannelProcessFeature.Query.SaleChannelOrderPreProcessor;
public class SaleChannelOrderPreProcessorCommand : IRequest<ServiceResultDTO>
{
  public int StoreId { get; set; }
  public int SaleChannelConfigId { get; set; }
  public DateTime CreatedFrom { get; set; }
  public DateTime CreatedTo { get; set; }
  public int OrderTypeId { get; set; } = (int)EnumOrderType.Regular;
}
