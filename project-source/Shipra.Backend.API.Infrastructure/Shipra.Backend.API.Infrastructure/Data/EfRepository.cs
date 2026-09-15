using Ardalis.Specification.EntityFrameworkCore;
using Shipra.Backend.API.SharedKernel.Interfaces;

namespace Shipra.Backend.API.Infrastructure.Data;

// inherit from Ardalis.Specification type
public class EfRepository<T> : RepositoryBase<T>, IReadRepository<T>, IRepository<T> where T : class, IAggregateRoot
{
  public EfRepository(AppDbContext dbContext) : base(dbContext)
  {
  }
}
