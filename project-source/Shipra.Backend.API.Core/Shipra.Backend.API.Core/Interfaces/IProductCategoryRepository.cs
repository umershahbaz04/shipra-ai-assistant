using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.ProductAggregate;
using Shipra.Backend.API.Core.StoresAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IProductCategoryRepository
{
  Task<dynamic> CreateProductCategory(ProductCategory model);
  Task<dynamic> UpdateProductCategory(ProductCategory model);
  Task<dynamic> DeleteProductCategoryById(ProductCategory model);
  Task<ProductCategory?> GetProductCategoryById(int productCategoryId);
  Task<List<ProductCategory>?> GetAllProductCategoryByClientId(ClientId clientId);
  Task<List<ProductCategory>?> GetAllProductCategoryLookupByClientId(ClientId clientId);
  Task<ProductCategory?> GetProductCategoryByName(string? categoryName, ClientId clientId);
   
}
