using Shipra.Backend.API.Application.DTOs.OrderUseCase.HelperOrder.UDTOrder;

namespace Shipra.Backend.API.Application.DTOs.ReturnReportUseCase;
public class UDTReturnReportDetailResponse
{
  public List<dynamic>? Data { get; set; }
  public bool IsSuccessed { get; set; }
  public IList<UDTFileUploadError>? Errors { get; set; }
}

