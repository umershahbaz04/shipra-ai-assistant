using DocumentFormat.OpenXml.Wordprocessing;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Core.NotificationAggregate;

public class NotificationConfig
{
  public NotificationConfigId? NotificationConfigId { get; set; }
  public string? Text { get; set; }
  public int? NotificationChannelId { get; set; } 
  public ClientId? ClientId { get; set; }
  public int? NotificationEventId { get; set; }
  public string? Config { get; set; }
  public EmployeeId? CreatedBy { get; set; }
  public DateTime? CreatedOn { get; set; }
  public EmployeeId? UpdatedBy { get; set; }
  public DateTime? UpdateOn { get; set; }
  public bool? Active { get; set; }

  public static NotificationConfig Create(string? text, int? notificationChannelId, ClientId? clientId, int? notificationEventId, string? config, EmployeeId? createdBy)
  {
    return new NotificationConfig()
    {
      NotificationConfigId = NotificationConfigId.New,
      Text = text,
      NotificationChannelId = notificationChannelId, 
      ClientId = clientId,
      NotificationEventId = notificationEventId,
      Config = config,
      CreatedBy = createdBy,
      CreatedOn = DateTime.UtcNow,
      Active = true
    };
  }

  public void Update(string? text, int? notificationChannelId, ClientId clientId, int? notificationEventId, string? config, EmployeeId employeeId)
  {
    Text = text;
    NotificationChannelId = notificationChannelId; 
    ClientId = clientId;
    NotificationEventId = notificationEventId;
    Config = config;
    UpdatedBy = employeeId;
    UpdateOn = DateTime.UtcNow; 
  } 
}
public sealed record NotificationConfigId(Guid Value)
{
  public static NotificationConfigId New => new(Guid.NewGuid());
}
