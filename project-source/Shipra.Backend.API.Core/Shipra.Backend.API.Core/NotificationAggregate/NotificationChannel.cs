namespace Shipra.Backend.API.Core.NotificationAggregate;

public class NotificationChannel
{
  public int NotificationChannelId { get; set; }
  /// <summary>
  /// ServiceTypeId refer to tables like activated sms,email etc
  /// </summary>
  public int? NotificationTypeId { get; set; }
  public int? ServiceTypeId { get; set; }

  public static NotificationChannel Create(int notificationTypeId, int serviceTypeId)
  {
    return new NotificationChannel { NotificationTypeId = notificationTypeId, ServiceTypeId = serviceTypeId };
  }

  public void Update(int notificationTypeId, int serviceTypeId)
  {
    NotificationTypeId = notificationTypeId;
    ServiceTypeId = serviceTypeId;
  }
}
