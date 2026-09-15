using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.ProductAggregate;
public class ProductCategoryLookup
{
  public int ProductCategoryId { get; private set; } 
  public string? Name { get; private set; }

  public static ProductCategoryLookup CreateProductCategory(string? name)
  {
    return new ProductCategoryLookup()
    {
      Name = name,
    }; 
  }

  public void UpdateProductCategory(string? name)
  {
    Name = name;
  }
}
