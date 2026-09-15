using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrder;

namespace Shipra.Backend.API.Application.DTOs.SaleChannelUseCase.WooCommereceOrder;
public class WoocommerceOrderDetailResponse
{
  public IList<CreateUploadOrderResponseModel>? WoocommerceOrderDetail { get; set; }
  public bool IsSuccessed { get; set; }
  public IList<WoocommerceOrderError>? Errors { get; set; }
}
