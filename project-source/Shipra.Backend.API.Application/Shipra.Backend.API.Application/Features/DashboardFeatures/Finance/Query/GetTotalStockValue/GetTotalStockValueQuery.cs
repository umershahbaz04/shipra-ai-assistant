using MediatR;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;

namespace Shipra.Backend.API.Application.Features.DashboardFeatures.Finance.Query.GetTotalStockValue;
public class GetTotalStockValueQuery : IRequest<ServiceResultDTO>
{
  public FilterDateClientModel? FilterModel { get; set; }
}
