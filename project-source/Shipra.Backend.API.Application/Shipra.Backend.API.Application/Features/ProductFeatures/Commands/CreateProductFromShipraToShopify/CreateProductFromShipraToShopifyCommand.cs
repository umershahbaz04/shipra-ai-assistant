using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Commands.CreateProductFromShipraToShopify;
public class CreateProductFromShipraToShopifyCommand : IRequest<ServiceResultDTO>
{
  public string? ProductIds { get; set; }
  public int SaleChannelConfigId { get; set; }
  public int? SaleChannelLookupId { get; set; }
  public int? StoreId { get; set; }
}
