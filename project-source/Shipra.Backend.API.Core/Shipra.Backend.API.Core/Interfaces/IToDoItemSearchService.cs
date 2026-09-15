using Ardalis.Result;
using Shipra.Backend.API.Core.ProjectAggregate;

namespace Shipra.Backend.API.Core.Interfaces;

public interface IToDoItemSearchService
{
  Task<Result<ToDoItem>> GetNextIncompleteItemAsync(int projectId);
  Task<Result<List<ToDoItem>>> GetAllIncompleteItemsAsync(int projectId, string searchString);
}
