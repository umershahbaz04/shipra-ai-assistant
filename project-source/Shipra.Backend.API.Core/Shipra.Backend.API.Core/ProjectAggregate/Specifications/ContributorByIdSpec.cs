using Ardalis.Specification;

namespace Shipra.Backend.API.Core.ContributorAggregate.Specifications;

public class ContributorByIdSpec : Specification<Contributor>, ISingleResultSpecification
{
  public ContributorByIdSpec(int contributorId)
  {
    //Query
    //    .Where(contributor => contributor.Id == contributorId);
  }
}
