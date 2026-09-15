using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.CatalougeAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Infrastructure.Data.Repository;
public class CatalogueRepository : ICatalogueRepository
{
  private readonly ShipraMasterDbContext _shipraMasterDbContext;

  public CatalogueRepository(ShipraMasterDbContext shipraMasterDbContext)
  {
    _shipraMasterDbContext = shipraMasterDbContext;
  }
  public async Task<List<CatalogueDatabase>> GetAllCatalogueDatabases()
  {
    var catalogue = await _shipraMasterDbContext.CatalogueDatabases.ToListAsync();
    return catalogue;
  }
  public async Task<bool> UpdateCatalogueDatabase(CatalogueDatabase catalogueDatabase)
  {
    _shipraMasterDbContext.CatalogueDatabases.Update(catalogueDatabase);
    return await _shipraMasterDbContext.SaveChangesAsync() > 0;
  }
  public async Task<bool> CreateCatalogue(Catalogue catalogue)
  {
    bool isSave = false;
    var oTarget = await _shipraMasterDbContext.Catalogues.FirstOrDefaultAsync(x => x.ClientId == catalogue.ClientId);
    if (oTarget is null)
    {
      var data = await _shipraMasterDbContext.Catalogues.AddAsync(catalogue);
      isSave = await _shipraMasterDbContext.SaveChangesAsync() > 0;
    }
    return isSave;
  }
  public async Task<List<Catalogue>> GetAllCatalogues()
  {
    return await _shipraMasterDbContext.Catalogues.ToListAsync();
  }
  public async Task<Catalogue?> GetCatalogByClientId(string? clientId)
  {
    var oCatalogue = await _shipraMasterDbContext.Catalogues.Where(x => x.ClientId == clientId).FirstOrDefaultAsync();//.Take((int)oOrder.ItemsCount!).ToListAsync();
    return oCatalogue!;
  }
}
