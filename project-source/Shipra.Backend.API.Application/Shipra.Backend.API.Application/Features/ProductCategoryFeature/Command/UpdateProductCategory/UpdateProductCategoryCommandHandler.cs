using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ProductCategoryFeature.Command.UpdateProductCategory;

public class UpdateProductCategoryCommandHandler : RequestHandlerBase<UpdateProductCategoryCommand, ServiceResultDTOWithTypeModel<BaseResponseDto>>
{
  private readonly IProductCategoryRepository _productCategoryRepository;

  public UpdateProductCategoryCommandHandler(IProductCategoryRepository productCategoryRepository, IServiceProvider serviceProvider, ILogger<UpdateProductCategoryCommandHandler> logger) : base(serviceProvider, logger)
  {
    _productCategoryRepository = productCategoryRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<BaseResponseDto>> HandleRequest(UpdateProductCategoryCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTOWithTypeModel<BaseResponseDto> serviceResult = new ServiceResultDTOWithTypeModel<BaseResponseDto>();

    try
    {
      if (!GuidHelper.Validator(_currentUser.ClientIdStr))
      {
        throw new InvalidIdTypeException(_currentUser.ClientIdStr!);
      }
      var productCategory = await _productCategoryRepository.GetProductCategoryById(request.ProductCategoryId);
      if (productCategory == null)
      {
        throw new EntityNotFoundException("Product CategoryWooComereceModal", request.ProductCategoryId);
      }
      productCategory!.UpdateProductCategory(request.ProductCategoryId, request.CategoryName, _currentUser.EmployeeId!);
      await _productCategoryRepository.UpdateProductCategory(productCategory);

      serviceResult.CreateSuccessResponse();
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }

  }
}


