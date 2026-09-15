using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Application.Features.ProductCategoryFeature.Command.CreateProductCategory;

public class CreateProductCategoryCommandHandler : RequestHandlerBase<CreateProductCategoryCommand, ServiceResultDTOWithTypeModel<BaseResponseDto>>
{
  private readonly IProductCategoryRepository _productCategoryRepository;

  public CreateProductCategoryCommandHandler(IProductCategoryRepository productCategoryRepository, IServiceProvider serviceProvider, ILogger<CreateProductCategoryCommandHandler> logger) : base(serviceProvider, logger)
  {
    _productCategoryRepository = productCategoryRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<BaseResponseDto>> HandleRequest(CreateProductCategoryCommand request, CancellationToken cancellationToken)
  {

    ServiceResultDTOWithTypeModel<BaseResponseDto> serviceResult = new ServiceResultDTOWithTypeModel<BaseResponseDto>();

    try
    {
      ProductCategory? oProductCategory = await _productCategoryRepository.GetProductCategoryByName(request.CategoryName,_currentUser.ClientId!);
      if (oProductCategory is null)
      { 
        var productCategory = ProductCategory.CreateProductCategory(request.CategoryName, _currentUser.ClientId!, _currentUser.EmployeeId!);

        await _productCategoryRepository.CreateProductCategory(productCategory);
        serviceResult.CreateSuccessResponse();
      }
      else
      {
        serviceResult.CreateError("AlreadyExist", new string[] { $"Product category with name {request.CategoryName} already exist" });
      }
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
