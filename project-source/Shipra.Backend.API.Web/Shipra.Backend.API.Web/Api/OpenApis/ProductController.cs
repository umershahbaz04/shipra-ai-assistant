using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Features.ProductCategoryFeature.Query.GetAllProductCategory;
using Shipra.Backend.API.Application.Features.ProductFeatures.Query.GetAllProducts;
using Shipra.Backend.API.Application.Features.ProductFeatures.Query.GetSellerProductByClientAndProductId;

namespace Shipra.Backend.API.Web.Api.OpenApis;

public class ProductController : BaseApiController
{
  public ProductController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }
  [HttpGet("GetSellerProductByClientAndProductId")]
  public async Task<ActionResult> GetSellerProductByClientAndProductId([FromQuery] GetSellerProductByClientAndProductIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("get-all-product-category")]
  public async Task<ActionResult> GetProductCategory(CancellationToken cancellationToken = default)
  {
    GetAllProductCategoryQuery request = new GetAllProductCategoryQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("get-all-product")]
  public async Task<ActionResult> GetAllProducts([FromBody] GetAllProductsQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
}
