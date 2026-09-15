using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.ClientAggregate;

namespace Shipra.Backend.API.Core.WebhookEventAggregate;
public partial class WebhookEventLog
{
  public long WebhookEventLogId { get; set; }
  public int? WebhookEventLookupId { get; set; }
  public bool? IsSuccess { get; set; }
  public string? Request { get; set; }
  public string? Response { get; set; }
  public int? StatusCode { get; set; }
  public string? StatusName { get; set; }
  public string? ErrorMessage { get; set; }
  public ClientId? ClientId { get; set; }
  public DateTime? CreatedOn { get; set; }
  public DateTime? UpdatedOn { get; set; }
  public bool? Active { get; set; }

  public static WebhookEventLog Create(int? webhookEventLookupId, bool? isSuccess, string? request, string? response, int? statusCode, string? statusName, string? errorMessage, ClientId clientId)
  {
    return new WebhookEventLog
    {
      WebhookEventLookupId = webhookEventLookupId,
      IsSuccess = isSuccess,
      Request = request,
      Response = response,
      StatusCode = statusCode,
      StatusName = statusName,
      ErrorMessage = errorMessage,
      ClientId = clientId,
      CreatedOn = DateTime.UtcNow
    };
  }

}
