using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;

namespace Shipra.Backend.API.Core.CarrierAggregate;
public partial class WebhookUrlHash
{
  public Guid WebhookUrlHashId { get; set; }
  public Guid? ClientId { get; set; }
  public int? CarrierId { get; set; }
  public int? ActiveCarrierId { get; set; }
  public int? CarrierContractTypeId { get; set; }
  public EmployeeId? CreatedBy { get; set; }
  public DateTime? CreatedOn { get; set; }

  public static WebhookUrlHash CreateWebhookUrlHash(int carrierId,int? activeCarrierId, Guid? clientId, EmployeeId? userId,int? carrierContractTypeId = (int)EnumCarrierContractType.OwnContractType)
  {
    return new WebhookUrlHash
    {
      WebhookUrlHashId = Guid.NewGuid(),
      CarrierId = carrierId,
      ActiveCarrierId = activeCarrierId,
      CarrierContractTypeId = carrierContractTypeId,
      ClientId = clientId,
      CreatedBy = userId,
      CreatedOn = DateTime.UtcNow
    };
  }
}
