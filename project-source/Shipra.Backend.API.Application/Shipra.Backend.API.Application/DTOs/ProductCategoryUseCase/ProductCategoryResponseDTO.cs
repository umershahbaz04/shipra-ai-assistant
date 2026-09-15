using Shipra.Backend.API.Application.DTOs.Common.Base.Response;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Application.DTOs.ProductCategoryUseCase;
public class ProductCategoryDTO<T> : ErrorResponse
{
  public T? Result { get; set; }
}
public class ProductCategoryResponseModel : ProductCategory
{ 
}
