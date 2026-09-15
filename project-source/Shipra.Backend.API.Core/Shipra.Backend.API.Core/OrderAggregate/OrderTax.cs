using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.OrderAggregate;
public class OrderTax
{
  public OrderTaxId? OrderTaxId { get; set; }
  public OrderId? OrderId { get; set; } 
  public int? TaxId { get; set; } 
  public decimal? TaxValue { get; set; } 
  public bool? Active { get; set; }

  public static OrderTax Create(int? taxId, decimal? taxValue,OrderId orderId)
  {
    return new OrderTax
    {
      OrderTaxId = OrderTaxId.New,
      TaxValue = taxValue,
      TaxId = taxId,
      OrderId = orderId,
      Active = true
    };
  }

  public void Update(decimal? taxValue)
  {
    TaxValue = taxValue;
  }
}
public sealed record OrderTaxId(Guid Value)
{
  public static OrderTaxId New => new(Guid.NewGuid());
}
