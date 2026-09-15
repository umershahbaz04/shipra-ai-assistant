using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Core.ReturnAggregate;
public class Return
{
  public ReturnId? ReturnId { get; set; }
  public ClientId? ClientId { get; set; }
  public OrderId? OrderId { get; set; }
  public int ClientReturnReasonId { get; set; }
  public string? ReturnComment { get; set; }
  public int ReturnStatusId { get; set; }
  public int OrderTypeId { get; set; }
  public decimal? ReturnCharges { get; set; }
  public OrderId? RtoorderId { get; set; }
  public OrderId? ExchangeOrderId { get; set; }
  public int? RefundTypeId { get; set; }
  public decimal RefundAmount { get; set; }
  public string? ImagePath { get; set; }

  public static Return Create(ClientId clientId,OrderId orderId, int clientReturnReasonId, string returnComment, int orderTypeId, decimal? returnCharges, decimal refundAmount,string? imagePath)
  {
    return new Return()
    {
      ReturnId = ReturnId.New,
      ClientId = clientId,
      OrderId = orderId,
      ClientReturnReasonId = clientReturnReasonId,
      ReturnComment = returnComment,
      ReturnStatusId = (int)EnumReturnStatus.Created,
      OrderTypeId = orderTypeId,
      ReturnCharges = returnCharges, 
      RefundAmount = refundAmount,
      ImagePath = imagePath,

    };
  }
  public void UpdateReturnStatus(int returnStatusId)
  {
    ReturnStatusId = returnStatusId;
  }  
  public void UpdateReturnStatusWithRtoorderId(int returnStatusId,OrderId orderId)
  {
    ReturnStatusId = returnStatusId;
    RtoorderId = orderId;
  }
}
public sealed record ReturnId(Guid Value)
{
  public static ReturnId New => new(Guid.NewGuid());
}
