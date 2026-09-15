using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.ShopifyFeature.Command.CreateShopifyConfig;
public class CreateShopifyConfigCommand : IRequest<ServiceResultDTO>
{
  public string? code { get; set; }
  public string? hmac { get; set; }
  public string? host { get; set; }
  public string? shop { get; set; }
  public string? state { get; set; }
  public decimal? timestamp { get; set; }
}
