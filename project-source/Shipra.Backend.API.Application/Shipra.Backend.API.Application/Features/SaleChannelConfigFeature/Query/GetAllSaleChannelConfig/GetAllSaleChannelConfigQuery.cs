using MediatR;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;

namespace Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Query.GetAllSaleChannelConfig;
public class GetAllSaleChannelConfigQuery : IRequest<ServiceResultDTO>
{  
  public FilterModelDTO? FilterModel { get; set; }
}
