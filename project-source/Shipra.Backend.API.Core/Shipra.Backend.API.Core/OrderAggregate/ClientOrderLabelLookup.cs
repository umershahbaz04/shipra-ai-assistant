using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.Constants;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Core.OrderAggregate;
public partial class ClientOrderLabelLookup
{
  public int ClientOrderLabelLookupId { get; set; } 
  public string? LabelName { get; set; }   
  public ClientId? ClientId { get; set; } 
  public string? ColorCode { get; set; } 
  public bool? Active { get; set; } 
  public DateTime? CreatedOn { get; set; } 
  public EmployeeId? CreatedBy { get; set; } 
  public DateTime? UpdatedOn { get; set; } 
  public EmployeeId? UpdatedBy { get; set; }

  public static ClientOrderLabelLookup Create(ClientId clientId,string? labelName, string? colorCode, EmployeeId employeeId)
  {
    return new ClientOrderLabelLookup
    { 
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

  public static ClientOrderLabelLookup AddDefault()
  {
    return new ClientOrderLabelLookup()
    {
      ClientOrderLabelLookupId = 0,
      LabelName = ShipraConstants.DropDownPlaceHolderName
    };
  }
}
