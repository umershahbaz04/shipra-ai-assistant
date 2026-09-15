using Shipra.Backend.API.Core.ContributorAggregate;
using Shipra.Backend.API.Core.ContributorAggregate.Specifications;
using Shipra.Backend.API.SharedKernel.Interfaces;
using FastEndpoints;

namespace Shipra.Backend.API.Web.Endpoints.ContributorEndpoints;

public class GetById : Endpoint<GetContributorByIdRequest, ContributorRecord>
{
  private readonly IRepository<Contributor> _repository;

  public GetById(IRepository<Contributor> repository)
  {
    _repository = repository;
  }

  public override void Configure()
  {
    Get(GetContributorByIdRequest.Route);
    AllowAnonymous();
    Options(x => x
      .WithTags("ContributorEndpoints"));
  }
  public override async Task HandleAsync(GetContributorByIdRequest request, 
    CancellationToken cancellationToken)
  {
    var spec = new ContributorByIdSpec(request.ContributorId);
    var entity = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (entity == null)
    {
      await SendNotFoundAsync();
      return;
    }

    var response = new ContributorRecord(1, entity.Name);

    await SendAsync(response);
  }
}
