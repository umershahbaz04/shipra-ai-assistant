using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.AppConfigAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class AppConfigRepository : IAppConfigRepository
{
  private readonly DapperAppDbContext _dapperAppDbContext;
  private readonly AppDbContext _context;

  public AppConfigRepository(DapperAppDbContext dapperAppDbContext, AppDbContext context)
  {
    _dapperAppDbContext = dapperAppDbContext;
    _context = context;
  }
  public async Task<AppConfig?> GetAppConfigByKey(string key)
  {
    return await _context.AppConfigs.Where(x => x.AppConfigKey == key && x.Active == true).FirstOrDefaultAsync()!;
  }
}
