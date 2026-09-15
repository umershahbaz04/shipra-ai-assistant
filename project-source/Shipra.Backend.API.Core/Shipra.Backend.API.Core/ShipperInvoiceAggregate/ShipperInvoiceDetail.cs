using System;
using System.Collections.Generic;

namespace Shipra.Backend.API.Core.ShipperInvoiceAggregate;

public partial class ShipperInvoiceDetail
{
  public long ShipperInvoiceDetailId { get; private set; }
  public Guid? OrderId { get; private set; }
  public decimal? Rate { get; private set; }
  public Guid? ClientId { get; private set; }
  public int? ShipperInvoiceId { get; private set; }
  public string? OrderNo { get; private set; } 

  // Optional: Factory (if you want consistency with other models)
  public static ShipperInvoiceDetail Create(
      Guid? orderId,
      decimal? rate,
      Guid? clientId,
      int? shipperInvoiceId,
      string? orderNo 
    )
  {
    return new ShipperInvoiceDetail
    {
      OrderId = orderId,
      Rate = rate,
      ClientId = clientId,
      ShipperInvoiceId = shipperInvoiceId,
      OrderNo = orderNo 
    };
  } 
}
