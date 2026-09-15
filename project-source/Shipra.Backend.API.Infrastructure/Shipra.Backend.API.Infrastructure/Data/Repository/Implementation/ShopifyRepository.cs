using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.SaleChannelConfigAggregate;
using Shipra.Backend.API.Core.ShopifyAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class ShopifyRepository : IShopifyRepository
{
  private readonly DapperAppDbContext _dapperAppDbContext;
  private readonly AppDbContext _context;

  public ShopifyRepository(DapperAppDbContext dapperAppDbContext, AppDbContext context)
  {
    _dapperAppDbContext = dapperAppDbContext;
    _context = context;
  }
  public async Task<ShopifyConfig> CreateShopifyConfig(ShopifyConfig shopifyConfig)
  {
    await _context.ShopifyConfigs.AddAsync(shopifyConfig);
    await _context.SaveChangesAsync();
    return shopifyConfig;
  }

  public async Task<bool> DeleteShopifyConfig(ShopifyConfig shopifyConfig)
  {
    _context.ShopifyConfigs.Update(shopifyConfig);
    return await _context.SaveChangesAsync() > 0;
  }

  public Task<dynamic> GetAllShopifyConfig(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string clientId)
  {
    throw new NotImplementedException();
  }

  public async Task<ShopifyConfig?> GetShopifyConfigById(int saleChannelConfigId, ClientId clientId)
  {
    return await _context.ShopifyConfigs.Where(x => x.SaleChannelConfigId == saleChannelConfigId && x.ClientId == clientId).FirstOrDefaultAsync();
  }

  public async Task<ShopifyConfig> UpdateShopifyConfig(ShopifyConfig shopifyConfig)
  {
    _context.ShopifyConfigs.Update(shopifyConfig);
    await _context.SaveChangesAsync();
    return shopifyConfig;
  }

  public async Task<ShopifyConfig?> GetShopifyConfigByClientId(int? SaleChannelConfigId, ClientId clientId)
  {
    return await _context.ShopifyConfigs.Where(x => x.ClientId == clientId && x.SaleChannelConfigId == SaleChannelConfigId && x.Active == true).FirstOrDefaultAsync();
  }
}
