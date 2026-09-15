using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Query.GetSaleChannelByStoreIdForSelection;
public class GetSaleChannelByStoreIdForSelectionQuery : IRequest<ServiceResultDTO>
{
  public int StoreId { get; set; }
}
