namespace Shipra.Backend.API.Application.DTOs.SaleChannelUseCase.WooCommereceOrder;
public class WoocommerceOrderError
{
  public bool IsSuccessed { get; set; }
  public int Row { get; set; }
  public List<string> Msg { get; set; } = new List<string>();
}
