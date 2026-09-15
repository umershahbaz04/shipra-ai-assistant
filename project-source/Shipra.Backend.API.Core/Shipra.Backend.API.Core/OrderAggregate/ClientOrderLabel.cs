using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Core.OrderAggregate;
public partial class ClientOrderLabel
{
  public ClientOrderLabelId? ClientOrderLabelId { get; set; }
  public OrderId? OrderId { get; set; }
  public string? LabelName { get; set; }
  public ClientId? ClientId { get; set; }
  public string? ColorCode { get; set; }
  public bool? Active { get; set; }
  public DateTime? CreatedOn { get; set; }
  public EmployeeId? CreatedBy { get; set; } 
  public DateTime? UpdatedOn { get; set; }
  public EmployeeId? UpdatedBy { get; set; }

  public static ClientOrderLabel Create(ClientId clientId, OrderId orderId, string? labelName, string? colorCode, EmployeeId employeeId)
  {
    return new ClientOrderLabel
    {
      ClientOrderLabelId = ClientOrderLabelId.New,
      OrderId = orderId,
      LabelName = labelName,
      ClientId = clientId,
      ColorCode = colorCode,
      Active = true,
      CreatedBy = employeeId
    };
  }
  public void Update(string? labelName, string? colorCode, EmployeeId employeeId)
  {
    LabelName = labelName; 
    ColorCode = colorCode;
    UpdatedOn = DateTime.UtcNow;
    UpdatedBy = employeeId;
  } 
  public void Delete(EmployeeId employeeId)
  {
    Active = false;
    UpdatedOn = DateTime.UtcNow;
    UpdatedBy = employeeId;
  }
}
public sealed record ClientOrderLabelId(Guid Value)
{
  public static ClientOrderLabelId New => new(Guid.NewGuid());
}
