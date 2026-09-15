using Ardalis.ApiEndpoints;
using Shipra.Backend.API.Core.ProjectAggregate;
using Shipra.Backend.API.Core.ProjectAggregate.Specifications;
using Shipra.Backend.API.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Shipra.Backend.API.Web.Endpoints.ProjectEndpoints;

public class GetById : EndpointBaseAsync
  .WithRequest<GetProjectByIdRequest>
  .WithActionResult<GetProjectByIdResponse>
{
  private readonly IRepository<Project> _repository;

  public GetById(IRepository<Project> repository)
  {
    _repository = repository;
  }

  [HttpGet(GetProjectByIdRequest.Route)]
  [SwaggerOperation(
    Summary = "Gets a single Project",
    Description = "Gets a single Project by Id",
    OperationId = "Projects.GetById",
    Tags = new[] { "ProjectEndpoints" })
  ]
  public override async Task<ActionResult<GetProjectByIdResponse>> HandleAsync(
    [FromRoute] GetProjectByIdRequest request,
    CancellationToken cancellationToken = new())
  {
    var spec = new ProjectByIdWithItemsSpec(request.ProjectId);
    var entity = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (entity == null)
    {
      return NotFound();
    }

    var response = new GetProjectByIdResponse
    (
      id: 1,
      name: entity.Name,
      items: entity.Items.Select(item => new ToDoItemRecord(1, item.Title, item.Description, item.IsDone))
        .ToList()
    );

    return Ok(response);
  }
}
