namespace Shipra.Backend.API.Core.ProductAggregate;
public class TransferProduct
{
  public TransferProduct() { }

  public long TransferProductsId { get; private set; }
  public ProductStaionTransferId? ProductStaionTransferId { get; set; }
  public string? ProductSku { get; set; }
  public ProductId? ProductId { get; set; }
  public int? Quantity { get; set; }
  public int? Accepted { get; set; }
  public int? Rejected { get; set; }


  public static TransferProduct CreateTransferProduct(ProductStaionTransferId? productStaionTransferId, string? productSku, ProductId? productId, int? quantity, int? accepted, int? rejected)
  {
    return new TransferProduct()
    {
      //TransferProductsId = new int(),
      ProductStaionTransferId = productStaionTransferId,
      ProductSku = productSku,
      ProductId = productId,
      Quantity = quantity,
      Accepted = accepted,
      Rejected = rejected,
    };
  }
}
