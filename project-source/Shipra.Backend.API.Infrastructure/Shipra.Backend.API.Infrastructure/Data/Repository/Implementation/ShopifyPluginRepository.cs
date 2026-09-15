using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.SaleChannelConfigAggregate;
using Shipra.Backend.API.Core.ShopifyAggregate;
using Shipra.Backend.API.Core.StoresAggregate;
using Shipra.Backend.API.Infrastructure.Services.Interface;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class ShopifyPluginRepository : IShopifyPluginRepository
{
  private readonly IDbContextService _dbContextService;

  public ShopifyPluginRepository(IDbContextService dbContextService)
  {
    _dbContextService = dbContextService;
  }
  public async Task<ShopifyConfig> CreateShopifyConfig(ShopifyConfig shopifyConfig)
  {
    using (var _context = _dbContextService.GetAppDbContext(shopifyConfig.ClientId!.Value.ToString()))
    {
      await _context.ShopifyConfigs.AddAsync(shopifyConfig);
      await _context.SaveChangesAsync();
      return shopifyConfig;
    }

  }

  public async Task<bool> DeleteShopifyConfig(ShopifyConfig shopifyConfig)
  {
    using (var _context = _dbContextService.GetAppDbContext(shopifyConfig.ClientId!.Value.ToString()))
    {
      _context.ShopifyConfigs.Update(shopifyConfig);
      return await _context.SaveChangesAsync() > 0;
    }
  }

  public Task<dynamic> GetAllShopifyConfig(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string clientId)
  {
    throw new NotImplementedException();
  }

  public async Task<ShopifyConfig?> GetShopifyConfigById(int saleChannelConfigId, ClientId clientId)
  {
    using (var _context = _dbContextService.GetAppDbContext(clientId!.Value.ToString()))
    {
      return await _context.ShopifyConfigs.Where(x => x.SaleChannelConfigId == saleChannelConfigId && x.ClientId == clientId).FirstOrDefaultAsync();
    }
  }

  public async Task<ShopifyConfig> UpdateShopifyConfig(ShopifyConfig shopifyConfig)
  {
    using (var _context = _dbContextService.GetAppDbContext(shopifyConfig.ClientId!.Value.ToString()))
    {
      _context.ShopifyConfigs.Update(shopifyConfig);
      await _context.SaveChangesAsync();
      return shopifyConfig;
    }
  }

  public async Task<ShopifyConfig?> GetShopifyConfigByClientId(int? SaleChannelConfigId, ClientId clientId)
  {
    using (var _context = _dbContextService.GetAppDbContext(clientId!.Value.ToString()))
    {
      return await _context.ShopifyConfigs.Where(x => x.ClientId == clientId && x.SaleChannelConfigId == SaleChannelConfigId && x.Active == true).FirstOrDefaultAsync();
    }
  }

  public async Task<List<Store>?> GetStoresForSelection(ClientId? clientId)
  {
    using (var _context = _dbContextService.GetAppDbContext(clientId!.Value.ToString()))
    {
      return await _context.Stores.Where(x => x.ClientId == clientId).ToListAsync();
    }
  }

  public async Task<SaleChannelConfig> UpdateSaleChannelConfig(SaleChannelConfig SaleChannelConfig)
  {
    using (var _context = _dbContextService.GetAppDbContext(SaleChannelConfig.ClientId!.Value.ToString()))
    {
      _context.SaleChannelConfigs.Update(SaleChannelConfig);
      await _context.SaveChangesAsync();
      return SaleChannelConfig;
    }
  }

  public async Task<SaleChannelConfig?> GetSaleChannelConfigForUpdateById(int SaleChannelConfigId, ClientId clientId)
  {
    using (var _context = _dbContextService.GetAppDbContext(clientId!.Value.ToString()))
    {
      return await _context.SaleChannelConfigs.FirstOrDefaultAsync(x => x.SaleChannelConfigId == SaleChannelConfigId && x.ClientId == clientId);
    }
  }

  public async Task<bool> DeleteSaleChannelConfig(SaleChannelConfig SaleChannelConfig)
  {
    using (var _context = _dbContextService.GetAppDbContext(SaleChannelConfig.ClientId!.Value.ToString()))
    {
      _context.SaleChannelConfigs.Update(SaleChannelConfig);
      return await _context.SaveChangesAsync() > 0;
    }
  }
  public async Task<Client?> GetClientById(ClientId clientId)
  {
    using (var _context = _dbContextService.GetAppDbContext(clientId!.Value.ToString()))
    {
      return await _context.Clients.FirstOrDefaultAsync(x => x.ClientId == clientId);
    }
  }
  public async Task<SaleChannelConfig> CreateSaleChannelConfig(SaleChannelConfig SaleChannelConfig)
  {
    using (var _context = _dbContextService.GetAppDbContext(SaleChannelConfig.ClientId!.Value.ToString()))
    {
      await _context.SaleChannelConfigs.AddAsync(SaleChannelConfig);
      await _context.SaveChangesAsync();
      return SaleChannelConfig;
    }
  }
  public async Task<SaleChannelConfig?> GetSaleChannelNameValidate(string? saleChannelName, ClientId clientId)
  {
    using (var _context = _dbContextService.GetAppDbContext(clientId!.Value.ToString()))
    {
      return await _context.SaleChannelConfigs.FirstOrDefaultAsync(x => x.SaleChannelName == saleChannelName!.ToLower().Trim() && x.ClientId == clientId);
    }
  }
  public async Task<SaleChannelConfig?> GetSaleChannelConfigByKey(string saleChannelKey, string clientId)
  {
    using (var _context = _dbContextService.GetAppDbContext(clientId))
    {
      return await _context.SaleChannelConfigs.Where(x => x.SaleChannelKey == saleChannelKey).FirstOrDefaultAsync();
    }
  }
}
