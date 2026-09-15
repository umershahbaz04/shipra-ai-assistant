using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;

 
public class ProductCategoryRepository : IProductCategoryRepository
{
  private readonly AppDbContext _context;

  public ProductCategoryRepository(AppDbContext context)
  {
    _context = context;
  }

  public async Task<dynamic> CreateProductCategory(ProductCategory model)
  {
    await _context.ProductCategories.AddAsync(model);
    await _context.SaveChangesAsync();
    return model;
  }

  public async Task<dynamic> DeleteProductCategoryById(ProductCategory model)
  {
    _context.ProductCategories.Update(model);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<List<ProductCategory>?> GetAllProductCategoryByClientId(ClientId clientId)
  {
    return await _context.ProductCategories.Where(x => x.ClientId == clientId && x.Active == true).ToListAsync();
  }
  public async Task<ProductCategory?> GetProductCategoryByName(string? categoryName, ClientId clientId)
  {
    return await _context.ProductCategories.FirstOrDefaultAsync(x => x.ClientId == clientId && x.Active == true && x.CategoryName!.Trim().ToLower() == categoryName!.Trim().ToLower());
  }
  public async Task<List<ProductCategory>?> GetAllProductCategoryLookupByClientId(ClientId clientId)
  {
    return await _context.ProductCategories.Where(x => x.ClientId == clientId && x.Active == true).ToListAsync();
  }
  public async Task<ProductCategory?> GetProductCategoryById(int ProductCategoryid)
  {
    return await _context.ProductCategories.FirstOrDefaultAsync(x => x.ProductCategoryId == ProductCategoryid && x.Active == true);
  }

  public async Task<dynamic> UpdateProductCategory(ProductCategory model)
  {
    _context.ProductCategories.Update(model);
    return await _context.SaveChangesAsync() > 0;
  }
}
