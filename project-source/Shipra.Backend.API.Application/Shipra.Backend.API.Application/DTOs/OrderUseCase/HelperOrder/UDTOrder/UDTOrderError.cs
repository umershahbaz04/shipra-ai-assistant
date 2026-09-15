namespace Shipra.Backend.API.Application.DTOs.OrderUseCase.HelperOrder.UDTOrder;

public class UDTFileUploadError
{
  public bool IsSuccessed { get; set; }
  public int Row { get; set; }
  public List<string> Msg { get; set; } = new List<string>();
}
