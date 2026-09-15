using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.Enum;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IConfigRepository
{
  Task<Mcconfig?> GetMcconfigByKey(string key, int? environmentTypeId = (int)EnumEnvironmentType.Live);
}
