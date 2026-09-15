using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Query.GetProductStockByProductStockId;
public class GetProductStockByProductStockIdForInventoryQuery : IRequest<ServiceResultDTO>
{
  public long ProductStockId { get; set; }
}
