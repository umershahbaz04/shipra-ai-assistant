using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Core.SaleChannelOrderAggregate;
public class SaleChannelOrder
{
  public SaleChannelOrder() { }
  public SaleChannelOrderId? SaleChannelOrderId { get; private set; }
  public int? SaleChannelLookupId { get; private set; }
  public int? SaleChannelConfigId { get; set; }
  public string? OrderId { get; private set; }
  public string? OrderNo { get; private set; }
  public string? OrderJson { get; private set; }
  public DateTime? OrderCreatedOn { get; private set; }
  public OrderId? ShipraOrderId { get; private set; }
  public ClientId? ClientId { get; private set; }
  public EmployeeId? CreatedBy { get; private set; }
  public DateTime? CreatedOn { get; private set; }
  public bool? Active { get; private set; }

  public static SaleChannelOrder CreateSaleChannelOrder(int? saleChannelLookupId, int? saleChannelConfigId, string? orderId, string? orderNo, string? orderJson, DateTime? orderCreatedOn, OrderId shipraOrderId, ClientId clientId, EmployeeId createdBy)
  {
    var SaleChannelOrder = new SaleChannelOrder()
    {
      SaleChannelOrderId = SaleChannelOrderId.New,
      SaleChannelLookupId = saleChannelLookupId,
      SaleChannelConfigId = saleChannelConfigId,
      OrderId = orderId,
      OrderNo = orderNo,
      OrderJson = orderJson,
      OrderCreatedOn = orderCreatedOn,
      ShipraOrderId = shipraOrderId,
      ClientId = clientId,
      CreatedBy = createdBy,
      CreatedOn = DateTime.UtcNow,
      Active = true
    };
    return SaleChannelOrder;
  }
}

public sealed record SaleChannelOrderId(Guid Value)
{
  public static SaleChannelOrderId New => new(Guid.NewGuid());
}
