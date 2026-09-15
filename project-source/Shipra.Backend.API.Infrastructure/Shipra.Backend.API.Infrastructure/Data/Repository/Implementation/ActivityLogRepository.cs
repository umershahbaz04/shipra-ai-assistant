using Shipra.Backend.API.Core.ActivityLogAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class ActivityLogRepository : IActivityLogRepository
{
  private readonly DapperAppDbContext _dapperAppDbContext;
  private readonly AppDbContext _context;

  public ActivityLogRepository(DapperAppDbContext dapperAppDbContext, AppDbContext context)
  {
    _dapperAppDbContext = dapperAppDbContext;
    _context = context;
  }

  public async Task<bool> Create(ActivityLog activityLog)
  {
    await _context.ActivityLogs.AddAsync(activityLog);
    return await _context.SaveChangesAsync() > 0;
  }
}
