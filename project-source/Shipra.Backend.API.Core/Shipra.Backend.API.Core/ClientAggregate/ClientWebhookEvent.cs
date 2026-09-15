using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.WebhookEventAggregate;

namespace Shipra.Backend.API.Core.ClientAggregate;
public partial class ClientWebhookEvent
{
  public long ClientWebhookEventId { get; set; } 
  public int? WebhookEventLookupId { get; set; }  
  public string? WebhookUrl { get; set; } 
  public ClientId? ClientId { get; set; } 
  public bool? IsActive { get; set; } 
  public EmployeeId? CreatedBy { get; set; } 
  public DateTime? CreatedOn { get; set; } 
  public EmployeeId? UpdatedBy { get; set; }

  public DateTime? UpdatedOn { get; set; }
  public static ClientWebhookEvent Create(int webhookEventLookupId,string? webhookUrl, ClientId? clientId, EmployeeId? createdBy)
  {
    return new ClientWebhookEvent
    {
      WebhookEventLookupId = webhookEventLookupId, 
      WebhookUrl = webhookUrl,
      ClientId = clientId,
      CreatedBy = createdBy,
      CreatedOn = DateTime.UtcNow
    };
  }

  public void Update(int webhookEventLookupId, string? webhookUrl,EmployeeId updatedById)
  {
    WebhookEventLookupId = webhookEventLookupId; 
    WebhookUrl = webhookUrl; 
    CreatedBy = updatedById;
    UpdatedOn = DateTime.UtcNow;
  } 
}
