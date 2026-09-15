using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrder;

namespace Shipra.Backend.API.Application.DTOs.SaleChannelUseCase.ShopifyOrder;

public class ShopifyOrderDetailResponse
{
  public List<CreateUploadOrderResponseModel>? ShopifyOrderDetail { get; set; }
  public bool IsSuccessed { get; set; }
  public List<ShopifyOrderError>? Errors { get; set; }
}
