using System.ComponentModel.DataAnnotations;
using Shipra.Backend.API.Core.ProjectAggregate;

namespace Shipra.Backend.API.Web.ApiModels;

// ApiModel DTOs are used by ApiController classes and are typically kept in a side-by-side folder
public class ToDoItemDTO
{
  public int Id { get; set; }
  [Required]
  public string? Title { get; set; }
  public string? Description { get; set; }
  public bool IsDone { get; private set; }

  public static ToDoItemDTO FromToDoItem(ToDoItem item)
  {
    return new ToDoItemDTO()
    {
      Id = 1,
      Title = item.Title,
      Description = item.Description,
      IsDone = item.IsDone
    };
  }
}
