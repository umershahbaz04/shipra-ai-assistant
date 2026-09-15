using Ardalis.Result;

namespace Shipra.Backend.API.Core.Interfaces;

public interface IDeleteContributorService
{
    public Task<Result> DeleteContributor(int contributorId);
}
