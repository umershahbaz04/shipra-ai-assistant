using Shipra.Backend.API.Core.ClientAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IClientRepository
{
  Task<dynamic> CreateClient(Client client);
  Task<dynamic> UpdateClient(Client client);
  Task<dynamic> DeleteClient(Client client);
  Task<Client?> GetClientById(ClientId clientId);
  Task<Client?> GetClientByIdWithConnectionstring(ClientId clientId);
  Task<dynamic?> GetAllClients(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir);
  Task<dynamic?> GetActiveClients(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir);
  Task<Client?> GetLastClient();
  Task<ClientAddress> CreateClientAddress(ClientAddress clientAddress);
  Task<dynamic> GetUserProfileInfo(string clientId);
  Task<dynamic> UpdateClientAddress(ClientAddress clientAddress);
  Task<ClientAddress?> GetClientAddresByClientId(ClientId? clientId);
  Task<Client?> CheckEmailExists(string? email);
  Task<dynamic> GetClientProfileById(string clientId);
  Task<Client?> GetClientByClientId(string clientId, string connectionString);
  Task<Client?> GetClientByKey(string? tenantUsername, string? publicKey, string? clientId);
  Task<ClientCarrierTrackingStatus> CreateClientCarrierTrackingStatus(ClientCarrierTrackingStatus oClientCarrierTrackingStatus);
  Task<List<ClientCarrierTrackingStatus>?> GetAllClientCarrierTrackingStatus(ClientId clientId);
  Task<bool> CreateBatchClientCarrierTrackingStatus(List<ClientCarrierTrackingStatus> listOfClientCarrier);
  Task<ClientAddress?> GetClientAddress(ClientId? clientId);
  Task<int> GetClientRegionMinutes(string? clientId);
  Task<ClientGenericSetting> ClientGenericSettingById(ClientId clientId);
  Task<bool> CreateClientGenericSetting(ClientGenericSetting model); 
  Task<bool> UpdateClientGenericSetting(ClientGenericSetting model); 
  Task<ClientGenericSettingLookup> GetClientGenericSettingLookup();
  Task<ClientGenericSetting> GetGenericSettingByClientIdAsync(ClientId clientId);
  Task<ClientConfigSetting> GetClientConfigSetting(ClientId clientId);
  Task<bool> UpdateClientConfigSetting(ClientConfigSetting clientSettingConfig);
  Task<bool> CreateClientConfigSetting(ClientConfigSetting clientSettingConfig);
  Task<bool> WipeOutClientData(ClientId clientId, Shipra.Backend.API.Core.Enum.EnumWipeOutSection section);
}

