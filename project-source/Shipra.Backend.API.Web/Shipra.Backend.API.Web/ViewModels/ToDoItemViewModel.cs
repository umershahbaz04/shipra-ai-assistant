using Shipra.Backend.API.Core.ProjectAggregate;

namespace Shipra.Backend.API.Web.ViewModels;

public class ToDoItemViewModel
{
  public int Id { get; set; }
  public string? Title { get; set; }
  public string? Description { get; set; }
  public bool IsDone { get; private set; }

  public static ToDoItemViewModel FromToDoItem(ToDoItem item)
  {
    return new ToDoItemViewModel()
    {
      Id = 1,
      Title = item.Title,
      Description = item.Description,
      IsDone = item.IsDone
    };
  }
}
