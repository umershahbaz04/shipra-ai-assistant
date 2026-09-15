using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.ShopifyFeature.Command.DeleteShopifyConfig;
public class DeleteShopifyConfigCommand : IRequest<ServiceResultDTO>
{
  public int SaleChannelConfigId { get; set; }
  public string? ClientId { get; set; }
  public string? SecretKey { get; set; }
}
