using Shipra.Backend.API.Core.ContributorAggregate;
using Shipra.Backend.API.SharedKernel.Interfaces;
using FastEndpoints;

namespace Shipra.Backend.API.Web.Endpoints.ContributorEndpoints;

public class Update : Endpoint<UpdateContributorRequest, UpdateContributorResponse>
{
  private readonly IRepository<Contributor> _repository;

  public Update(IRepository<Contributor> repository)
  {
    _repository = repository;
  }

  public override void Configure()
  {
    Put(CreateContributorRequest.Route);
    AllowAnonymous();
    Options(x => x
      .WithTags("ContributorEndpoints"));
  }
  public override async Task HandleAsync(
    UpdateContributorRequest request,
    CancellationToken cancellationToken)
  {
    if (request.Name == null)
    {
      ThrowError("Name is required");
    }

    var existingContributor = await _repository.GetByIdAsync(request.Id, cancellationToken);
    if (existingContributor == null)
    {
      await SendNotFoundAsync();
      return;
    }

    existingContributor.UpdateName(request.Name);

    await _repository.UpdateAsync(existingContributor, cancellationToken);

    var response = new UpdateContributorResponse(
        contributor: new ContributorRecord(1, existingContributor.Name)
    );

    await SendAsync(response);
  }
}
