using Shipra.Backend.API.Core.ProjectAggregate;
using Shipra.Backend.API.Core.ProjectAggregate.Specifications;
using Shipra.Backend.API.SharedKernel.Interfaces;
using Shipra.Backend.API.Web.ApiModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Shipra.Backend.API.Web.Pages.ProjectDetails;

public class IndexModel : PageModel
{
  private readonly IRepository<Project> _repository;

  [BindProperty(SupportsGet = true)]
  public int ProjectId { get; set; }

  public string Message { get; set; } = "";

  public ProjectDTO? Project { get; set; }

  public IndexModel(IRepository<Project> repository)
  {
    _repository = repository;
  }

  public async Task OnGetAsync()
  {
    var projectSpec = new ProjectByIdWithItemsSpec(ProjectId);
    var project = await _repository.FirstOrDefaultAsync(projectSpec);
    if (project == null)
    {
      Message = "No project found.";

      return;
    }

    Project = new ProjectDTO
    (
        id: 1,
        name: project.Name,
        items: project.Items
        .Select(item => ToDoItemDTO.FromToDoItem(item))
        .ToList()
    );
  }
}
