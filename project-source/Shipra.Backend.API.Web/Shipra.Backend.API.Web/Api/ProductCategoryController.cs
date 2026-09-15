using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Common.Security;
using Shipra.Backend.API.Application.Features.ProductCategoryFeature.Command.CreateProductCategory;
using Shipra.Backend.API.Application.Features.ProductCategoryFeature.Command.DeleteProductCategoryCommand;
using Shipra.Backend.API.Application.Features.ProductCategoryFeature.Command.UpdateProductCategory;
using Shipra.Backend.API.Application.Features.ProductCategoryFeature.Query.GetAllProductCategory;
using Shipra.Backend.API.Application.Features.ProductCategoryFeature.Query.GetAllProductCategoryLookup;
using Shipra.Backend.API.Application.Features.ProductCategoryFeature.Query.GetProductCategoryById;

namespace Shipra.Backend.API.Web.Api;
[Authorize]
public class ProductCategoryController : BaseApiController
{
  public ProductCategoryController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }
  [HttpGet("GetAllProductCategoryLookup")]
  public async Task<ActionResult> GetAllProductCategoryLookup(CancellationToken cancellationToken = default)
  {
    GetAllProductCategoryLookupQuery request = new GetAllProductCategoryLookupQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response); 
  } 
  [HttpGet("GetAllProductCategory")]
  public async Task<ActionResult> GetProductCategory(CancellationToken cancellationToken = default)
  {
    GetAllProductCategoryQuery request = new GetAllProductCategoryQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response); 
  }
  [HttpPost("CreateProductCategory")]
  public async Task<ActionResult> CreateProductCategory([FromBody] CreateProductCategoryCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("UpdateProductCategory")]
  public async Task<ActionResult> UpdateProductCategory([FromBody] UpdateProductCategoryCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("DeleteProductCategoryById")]
  public async Task<ActionResult> DeleteProductCategoryById([FromBody] DeleteProductCategoryCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetProductCategoryById")]
  public async Task<ActionResult> GetProductCategoryById([FromQuery] GetProductCategoryByIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

}
