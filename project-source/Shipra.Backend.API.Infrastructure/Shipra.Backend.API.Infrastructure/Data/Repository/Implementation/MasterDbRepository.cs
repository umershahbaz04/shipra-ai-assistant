using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.CatalougeAggregate;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class MasterDbRepository : IMasterDbRepository
{
  private readonly ShipraMasterDbContext _context;

  public MasterDbRepository(ShipraMasterDbContext context)
  {
    _context = context;
  }
  public async Task<Catalogue?> GetCatalogueByClientId(string clientId)
  {
   return await _context.Catalogues.FirstOrDefaultAsync(x => x.ClientId == clientId);
  } 
  public async Task<ShopifySession?> GetShopifySessionsByShop(string shop)
  {
   var data = await _context.ShopifySessions.FirstOrDefaultAsync(x => EF.Functions.Like(x.Shop!, shop));
    return data;
  }
}
