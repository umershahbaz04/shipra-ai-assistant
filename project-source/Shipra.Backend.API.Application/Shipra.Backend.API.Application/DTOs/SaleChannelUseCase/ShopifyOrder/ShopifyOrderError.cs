namespace Shipra.Backend.API.Application.DTOs.SaleChannelUseCase.ShopifyOrder;

public class ShopifyOrderError
{
  public bool IsSuccessed { get; set; }
  public int Row { get; set; }
  public List<string> Msg { get; set; } = new List<string>();
}
