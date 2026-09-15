using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrder;

namespace Shipra.Backend.API.Application.DTOs.OrderUseCase.HelperOrder.UDTOrder;

public class UDTOrderDetailResponse
{
  public IList<CreateUploadOrderResponseModel>? UDTOrderDetail { get; set; }
  public bool IsSuccessed { get; set; }
  public IList<UDTFileUploadError>? Errors { get; set; }
}
