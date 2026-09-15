using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Core.ReturnAggregate;
public class ReturnProduct
{
  public ReturnProductId? ReturnProductId { get; set; } 
  public ReturnId? ReturnId { get; set; } 
  public ProductId? ProductId { get; set; }
  public long? ProductStockId { get; set; }
  public decimal? ItemValue { get; set; }

  public static ReturnProduct Create(ReturnId? returnId, ProductId? productId, int? productStockId , decimal? itemValue)
  {
    return new ReturnProduct
    {
      ReturnProductId = ReturnProductId.New,
      ReturnId = returnId,
      ProductId = productId,
      ProductStockId = productStockId,
      ItemValue = itemValue
    };
  }
}
public sealed record ReturnProductId(Guid Value)
{
  public static ReturnProductId New => new(Guid.NewGuid());
}
