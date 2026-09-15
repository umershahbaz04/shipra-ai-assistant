

using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Models;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IClientLoginRepository
{
  Task<ClientResponseModel?> GetClientByClientId(string clientId, string connectionString);
  Task<EmployeeResponseModel?> GetEmployeebyId(string clientId, string employeeId, string connectionString);
  Task<ClientConfigSettingDto?> GetClientConfigSetting(string clientId, string connectionString);
}
