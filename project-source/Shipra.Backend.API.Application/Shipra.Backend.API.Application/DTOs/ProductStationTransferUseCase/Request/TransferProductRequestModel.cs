namespace Shipra.Backend.API.Application.DTOs.ProductStationTransferUseCase.Request;
public class TransferProductRequestModel
{
  public long TransferProductsId { get; set; }
  public string? ProductStaionTransferId { get; set; }
  public string? ProductSku { get; set; }
  public string? ProductId { get; set; }
  public int? Quantity { get; set; }
  public int? Accepted { get; set; }
  public int? Rejected { get; set; }
}
