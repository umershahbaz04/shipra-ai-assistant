using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.NotificationAggregate;
using Shipra.Backend.API.Core.WebhookEventAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IWebHookEventRepository
{ 
  Task<dynamic>? GetAllClientWebhookEndpoints(string? clientId); 

  Task<bool?> CreateClientWebhookEvent(ClientWebhookEvent model);
  Task<ClientWebhookEvent?> IsClientWebhookEventExist(ClientWebhookEvent model);
  Task<bool> UpdateClientWebhookEvent(ClientWebhookEvent model);
  Task<ClientWebhookEvent?> GetClientWebhookEventById(long clientWebhookEventId); 
  Task<List<WebhookEventLookup>> GetAllWebhookEventLookups();
  Task<ClientWebhookEvent?> GetClientWebhookEventById(int clientWebhookEventId); 
  Task<bool?> DeleteWebhookEvent(ClientWebhookEvent model);
  Task<dynamic> GetAllWebhookEventLogByClient(DateTime? createdFrom, DateTime? createdTo, int start, int length, string? search, int sortCol, string? sortDir, string clientId);
  Task<bool?> CreateWebHookEventlog(WebhookEventLog model);
  Task<WebhookEventLookup?> GetWebHookEventsByName(string? eventName);
  Task<dynamic> GetAllClientWebhookEvents(string clientId);
  Task<List<ClientWebhookEvent>> GetAllClientWebhookEvents(ClientId clientId);

}
