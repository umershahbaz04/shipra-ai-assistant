using Ardalis.Specification;
using Shipra.Backend.API.Core.ProjectAggregate;

namespace Shipra.Backend.API.Core.ProjectAggregate.Specifications;

public class ProjectByIdWithItemsSpec : Specification<Project>, ISingleResultSpecification
{
  public ProjectByIdWithItemsSpec(int projectId)
  {
    //Query
    //    .Where(project => project.Id == projectId)
    //    .Include(project => project.Items);
  }
}
