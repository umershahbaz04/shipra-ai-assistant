using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class OrderTypeLookupRepository : IOrderTypeLookupRepository
{
  private readonly AppDbContext _context;

  public OrderTypeLookupRepository(AppDbContext context)
  {
    _context = context;
  }
  public async Task<List<OrderTypeLookup>?> GetAllOrderTypeLookup()
  {
    return await _context.OrderTypeLookups.ToListAsync();
  }
}
