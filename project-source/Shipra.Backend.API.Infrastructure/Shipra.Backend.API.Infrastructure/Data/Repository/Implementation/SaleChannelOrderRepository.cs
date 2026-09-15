using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.SaleChannelOrderAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class SaleChannelOrderRepository : ISaleChannelOrderRepository
{
  private readonly DapperAppDbContext _dapperAppDbContext;
  private readonly AppDbContext _context;

  public SaleChannelOrderRepository(DapperAppDbContext dapperAppDbContext, AppDbContext context)
  {
    _dapperAppDbContext = dapperAppDbContext;
    _context = context;
  }
  public async Task<SaleChannelOrder> CreateSaleChannelOrder(SaleChannelOrder saleChannelOrder)
  {
    await _context.SaleChannelOrders.AddAsync(saleChannelOrder);
    await _context.SaveChangesAsync();
    return saleChannelOrder;
  }

  public async Task<List<SaleChannelOrder>?> GetAllSaleChannelOrder(ClientId clientId, int saleChannelLookupId)
  {
    return await _context.SaleChannelOrders.Where(x => x.SaleChannelLookupId == saleChannelLookupId && x.ClientId == clientId).ToListAsync();
  }
  public async Task<List<SaleChannelOrder>?> GetSaleChannelOrderListByOrderIds(string? orderIds, int saleChannelLookUpId, ClientId clientId)
  {
    var list = await _context.SaleChannelOrders.Where(x => orderIds!.Contains(x.OrderId!) && x.OrderId != "" && x.SaleChannelLookupId == saleChannelLookUpId && x.ClientId == clientId).ToListAsync();
    return list;
  }
}
