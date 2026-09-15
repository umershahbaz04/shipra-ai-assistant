using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Core.OrderAggregate;
public class OrderDraft
{
  public long OrderDraftId { get; private set; }
  public string? OrderNo { get; private set; }
  public string? OrderInfo { get;  set; }
  public int OrderTypeId { get; private set; }
  public DateTime? CreatedOn { get; private set; }
  public ClientId? ClientId { get; private set; }
  public EmployeeId? CreatedBy { get; private set; }
  public DateTime? UpdatedOn { get; private set; }
  public EmployeeId? UpdatedBy { get; private set; }
   

  // 🔹 Factory method for creating a new OrderDraft
  public static OrderDraft Create(string orderNo, string orderInfo, int orderTypeId, ClientId? clientId, EmployeeId? createdBy)
  { 
    return new OrderDraft
    {
      OrderNo = orderNo,
      OrderInfo = orderInfo,
      OrderTypeId = orderTypeId,
      ClientId = clientId,
      CreatedBy = createdBy,
      CreatedOn = DateTime.UtcNow
    };
  }

  // 🔹 Update method for modifying existing draft
  public void Update(string orderInfo, EmployeeId? createdBy)
  { 
    OrderInfo = orderInfo;
    UpdatedOn = DateTime.UtcNow;
    UpdatedBy = createdBy;
  }
  public string GetOrderNo(string? ordrNo)
  {
    return ordrNo!;
  }

  public static string GetOrderNo(OrderDraft firstODraft)
  {
    string newOrderNo = "#1"; // default if no previous order exists

    if (firstODraft != null && !string.IsNullOrEmpty(firstODraft.OrderNo))
    {
      // Extract numeric part (removes any non-digit characters)
      var numericPart = new string(firstODraft.OrderNo.Where(char.IsDigit).ToArray());

      if (int.TryParse(numericPart, out int currentNumber))
      {
        int nextNumber = currentNumber + 1;
        newOrderNo = $"#{nextNumber}";
      }
    }
    return newOrderNo;
  }
}
