using Shipra.Backend.API.Core.AppConfigAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IAppConfigRepository
{
  Task<AppConfig?> GetAppConfigByKey(string key);
}
