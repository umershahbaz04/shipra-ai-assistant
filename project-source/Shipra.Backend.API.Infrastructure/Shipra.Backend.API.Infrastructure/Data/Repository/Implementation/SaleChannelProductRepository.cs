using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.SaleChannelProductAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class SaleChannelProductRepository : ISaleChannelProductRepository
{
  private readonly DapperAppDbContext _dapperAppDbContext;
  private readonly AppDbContext _context;

  public SaleChannelProductRepository(DapperAppDbContext dapperAppDbContext, AppDbContext context)
  {
    _dapperAppDbContext = dapperAppDbContext;
    _context = context;
  }
  public async Task<SaleChannelProduct> CreateSaleChannelProduct(SaleChannelProduct saleChannelProduct)
  {
    await _context.SaleChannelProducts.AddAsync(saleChannelProduct);
    await _context.SaveChangesAsync();
    return saleChannelProduct;
  }

  public async Task<List<SaleChannelProduct>?> GetAllSaleChannelProduct(ClientId clientId, int? saleChannelLookupId)
  {
    return await _context.SaleChannelProducts.Where(x => x.SaleChannelLookupId == saleChannelLookupId && x.ClientId == clientId).ToListAsync();
  }

  public async Task<List<SaleChannelProduct>?> GetSaleChannelProductListByProductIds(string? productIds, int? saleChannelLookUpId, ClientId clientId)
  {
    var list = await _context.SaleChannelProducts.Where(x => productIds!.Contains(x.ProductId!) && x.ProductId != "" && x.SaleChannelLookupId == saleChannelLookUpId && x.ClientId == clientId).ToListAsync();
    return list;
  }
}
