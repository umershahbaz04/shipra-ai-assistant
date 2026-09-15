using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrderFromSaleChannelOrders;
public class CreateOrderFromSaleChannelOrdersCommand : IRequest<ServiceResultDTO>
{
  public string? OrderIds { get; set; }
  public int SaleChannelLookupId { get; set; }
}
