using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.NotificationAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface INotificationRepository
{
  Task<List<NotificationType>> GetNotificationTypes();
  Task<List<NotificationEvent>> GetNotificationEvents();
  Task<bool> CreaetNotificationConfig(NotificationConfig notificationConfig);
  Task<bool> UpdateNotificationConfig(NotificationConfig notificationConfig);
  Task<NotificationConfig> GetNotificationConfigById(NotificationConfigId notificationConfigId, ClientId? clientId);
  Task<List<NotificationConfig>> GetNotificationConfigByClientId(ClientId? clientId);
  Task<dynamic> GetNotificationConfigByTypeId(int notificationTypeId, ClientId? clientId);
  Task<bool> DeleteNotificationConfig(NotificationConfig notificationConfig);
  Task<bool> CreateNotificationChannel(NotificationChannel notificationChannel);
  Task<bool> UpdateNotificationChannel(NotificationChannel notificationChannel);
  Task<NotificationChannel> GetNotificationChannelBySCId(int notificationChannelId);
  Task<bool> DeleteNotificationChannel(NotificationChannel notificationChannel);
  Task<NotificationEvent?> GetNotificationEventsByName(string? eventName);

  #region  WhatsApp Button Handler
  Task<NotificationConfig> GetSingleNotificationConfigByClientId(ClientId? clientId);
  #endregion
}
