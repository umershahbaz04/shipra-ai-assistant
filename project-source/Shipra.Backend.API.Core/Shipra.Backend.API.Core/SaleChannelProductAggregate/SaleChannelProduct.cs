using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Core.SaleChannelProductAggregate;
public class SaleChannelProduct
{
  public SaleChannelProduct() { }
  public SaleChannelProductId? SaleChannelProductId { get; private set; }
  public int? SaleChannelLookupId { get; private set; }
  public string? ProductId { get; private set; }
  public string? ProductNo { get; private set; }
  public string? ProductJson { get; private set; }
  public DateTime? ProductCreatedOn { get; private set; }
  public ProductId? ShipraProductId { get; private set; }
  public ClientId? ClientId { get; private set; }
  public EmployeeId? CreatedBy { get; private set; }
  public DateTime? CreatedOn { get; private set; }
  public bool? Active { get; private set; }

  public static SaleChannelProduct CreateSaleChannelProduct(int? saleChannelLookupId, string? productId, string productNo, string? productJson, DateTime? productCreatedOn, ProductId shipraProductId, ClientId clientId, EmployeeId createdBy)
  {
    var SaleChannelProduct = new SaleChannelProduct()
    {
      SaleChannelProductId = SaleChannelProductId.New,
      SaleChannelLookupId = saleChannelLookupId,
      ProductId = productId,
      ProductNo = productNo,
      ProductJson = productJson,
      ProductCreatedOn = productCreatedOn,
      ShipraProductId = shipraProductId,
      ClientId = clientId,
      CreatedBy = createdBy,
      CreatedOn = DateTime.UtcNow,
      Active = true
    };
    return SaleChannelProduct;
  }
}

public sealed record SaleChannelProductId(Guid Value)
{
  public static SaleChannelProductId New => new(Guid.NewGuid());
}
