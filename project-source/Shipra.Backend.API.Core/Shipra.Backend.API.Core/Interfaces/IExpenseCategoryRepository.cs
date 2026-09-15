using Shipra.Backend.API.Core.ClientAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IExpenseCategoryRepository
{
  Task<int> CreateExpenseCategoryForGeneralSetting(ClientId? clientId);
}
