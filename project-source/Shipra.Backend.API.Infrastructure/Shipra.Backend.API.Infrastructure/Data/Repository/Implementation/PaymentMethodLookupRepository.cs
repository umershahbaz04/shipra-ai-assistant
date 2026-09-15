using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class PaymentMethodLookupRepository : IPaymentMethodLookupRepository
{
  private readonly AppDbContext _context;

  public PaymentMethodLookupRepository(AppDbContext context)
  {
    _context = context;
  }
  public async Task<List<PaymentMethodLookup>?> GetAllPaymentMethodLookup()
  {
    return await _context.PaymentMethodLookups.ToListAsync();
  }
}
