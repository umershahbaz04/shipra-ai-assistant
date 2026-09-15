using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Query.GetAllSaleChannelByLookupIdForSelection;
public class GetAllSaleChannelByLookupIdForSelectionQuery : IRequest<ServiceResultDTO>
{
  public int SaleChannelLookupId { get; set; }
}
