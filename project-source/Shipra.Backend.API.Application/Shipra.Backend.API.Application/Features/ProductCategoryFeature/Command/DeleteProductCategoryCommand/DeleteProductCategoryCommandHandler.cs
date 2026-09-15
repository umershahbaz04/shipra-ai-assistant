using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Application.Features.ProductCategoryFeature.Command.DeleteProductCategoryCommand;
public class DeleteProductCategoryCommandHandler : RequestHandlerBase<DeleteProductCategoryCommand, ServiceResultDTOWithTypeModel<BaseResponseDto>>
{
  private readonly IProductCategoryRepository _productCategoryRepository;
  public DeleteProductCategoryCommandHandler(IProductCategoryRepository productCategoryRepository, IServiceProvider serviceProvider, ILogger<DeleteProductCategoryCommandHandler> logger) : base(serviceProvider, logger)
  {
    _productCategoryRepository = productCategoryRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<BaseResponseDto>> HandleRequest(DeleteProductCategoryCommand request, CancellationToken cancellationToken)
  {
    var response = new ServiceResultDTOWithTypeModel<BaseResponseDto>();

    try
    {
      if (!GuidHelper.Validator(_currentUser.ClientIdStr))
      {
        throw new InvalidIdTypeException(_currentUser.ClientIdStr!);
      }
      var target = await _productCategoryRepository.GetProductCategoryById(request.ProductCategoryId);
      if (target == null)
      {
        throw new EntityNotFoundException("Product CategoryWooComereceModal", request.ProductCategoryId);
      }
      target!.DeleteProductCategory(request.ProductCategoryId, _currentUser.EmployeeId!);
      response.IsSuccess = await _productCategoryRepository.DeleteProductCategoryById(target);

      return response;
    }
    catch (Exception ex)
    {
      response.CreateErrorResponse(ex);
      throw;
    }

  }
}
