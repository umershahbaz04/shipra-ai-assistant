using System.Net;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ProductCategoryUseCase;
using Shipra.Backend.API.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common.Exceptions;


namespace Shipra.Backend.API.Application.Features.ProductCategoryFeature.Query.GetProductCategoryById;
public class GetProductCategoryByIdQueryHandler : RequestHandlerBase<GetProductCategoryByIdQuery,
ServiceResultDTOWithTypeModel<ProductCategoryResponseModel>>
{
  private readonly IProductCategoryRepository _productCategoryRepository;

  public GetProductCategoryByIdQueryHandler(IProductCategoryRepository productCategoryReository, IServiceProvider serviceProvider, ILogger<GetProductCategoryByIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _productCategoryRepository = productCategoryReository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<ProductCategoryResponseModel>> HandleRequest(GetProductCategoryByIdQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTOWithTypeModel<ProductCategoryResponseModel>();
    try
    {
      var target = await _productCategoryRepository.GetProductCategoryById(request.ProductCategoryId);
      if (target == null)
      {
        throw new EntityNotFoundException("Product CategoryWooComereceModal", request.ProductCategoryId);
      }

      var model = new ProductCategoryResponseModel()
      {
        ProductCategoryId = target.ProductCategoryId,
        CategoryName = target.CategoryName,
        Active = target.Active,
        ClientId = target.ClientId,
        CreatedBy = target.CreatedBy,
        CreatedOn = target.CreatedOn,
        UpdatedBy = target.UpdatedBy,
        UpdatedOn = target.UpdatedOn
      };

      serviceResult = new ServiceResultDTOWithTypeModel<ProductCategoryResponseModel>(model);
      serviceResult.CreateSuccessResponse(HttpStatusCode.OK);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }

  }
}
