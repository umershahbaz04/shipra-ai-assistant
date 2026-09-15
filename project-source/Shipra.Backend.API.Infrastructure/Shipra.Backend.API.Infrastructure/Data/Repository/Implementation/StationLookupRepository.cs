using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class StationLookupRepository : IStationLookupRepository
{
  private readonly AppDbContext _context;

  public StationLookupRepository(AppDbContext context)
  {
    _context = context;
  }

  public async Task<List<ProductStation>?> GetAllStationLookup(ClientId clientId)
  {
    return await _context.ProductStations.Where(x => x.ClientId == clientId && x.Active == true).ToListAsync();
  }
}
