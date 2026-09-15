using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.Constants;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Core.ReturnAggregate;
public class ClientReturnReason
{
  public int ClientReturnReasonId { get; private set; }
  public string? Reason { get; private set; }
  public string? ReasonDetail { get; private set; }
  public ClientId? ClientId { get; private set; }
  public EmployeeId? CreatedBy { get; set; }
  public DateTime? CreatedOn { get; set; }
  public EmployeeId? UpdatedBy { get; set; }
  public DateTime? UpdatedOn { get; set; }
  public bool? Active { get; set; }
  public static ClientReturnReason Create(string reason, string reasonDetail, ClientId clientId, EmployeeId createdBy)
  {
    return new ClientReturnReason
    {
      Reason = reason,
      ReasonDetail = reasonDetail,
      ClientId = clientId,
      CreatedBy = createdBy,
      CreatedOn = DateTime.UtcNow,
      Active = true
    };
  }
  public void Update(string reason, string reasonDetail, ClientId clientId, EmployeeId updatedBy)
  {
    Reason = reason;
    ReasonDetail = reasonDetail;
    ClientId = clientId;
    UpdatedBy = updatedBy;
    UpdatedOn = DateTime.UtcNow;
    Active = true;
  }
  public void ActivateDeactive(EmployeeId employeeId, bool isActive)
  {
    Active = isActive;
    UpdatedBy = employeeId;
    UpdatedOn = DateTime.UtcNow;
  }

  public static ClientReturnReason AddDefault()
  {
    return new ClientReturnReason()
    {
      ClientReturnReasonId = 0,
      Reason = ShipraConstants.DropDownPlaceHolderName
    };
  }
}

