using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Infrastructure.Data.Repository;
public class ConfigRepository : IConfigRepository
{
  private readonly ShipraMasterDbContext _context;

  public ConfigRepository(ShipraMasterDbContext context)
  {
    _context = context;
  }

  public async Task<Mcconfig?> GetMcconfigByKey(string key, int? environmentTypeId = (int)EnumEnvironmentType.Live)
  {
    return await _context.Mcconfigs.FirstOrDefaultAsync(x => x.Parameter!.Trim().ToLower() == key.Trim().ToLower() && x.EnvironmentTypeId == environmentTypeId);
  }
}
