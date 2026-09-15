using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ProductCategoryUseCase;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ProductCategoryFeature.Query.GetAllProductCategory;
public class GetAllProductCategoryQuery : IRequest<ServiceResultDTOWithTypeModel<List<ProductCategoryResponseModel>>>
{
}
public class GetAllProductCategoryQueryHandler : RequestHandlerBase<GetAllProductCategoryQuery, ServiceResultDTOWithTypeModel<List<ProductCategoryResponseModel>>>
{
  private readonly IProductCategoryRepository _productCategoryRepository;

  public GetAllProductCategoryQueryHandler(IProductCategoryRepository productCategoryRepository, IServiceProvider serviceProvider, ILogger<GetAllProductCategoryQueryHandler> logger) : base(serviceProvider, logger)
  {
    _productCategoryRepository = productCategoryRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<List<ProductCategoryResponseModel>>> HandleRequest(GetAllProductCategoryQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTOWithTypeModel<List<ProductCategoryResponseModel>> serviceResult = new ServiceResultDTOWithTypeModel<List<ProductCategoryResponseModel>>();

    try
    {
      if (!GuidHelper.Validator(_currentUser.ClientIdStr))
      {
        throw new InvalidIdTypeException(_currentUser.ClientIdStr!);
      }

      var data = await _productCategoryRepository.GetAllProductCategoryByClientId(_currentUser.ClientId!);
      var responseDto = _mapper.Map<List<ProductCategoryResponseModel>>(data);

      serviceResult = new ServiceResultDTOWithTypeModel<List<ProductCategoryResponseModel>>(responseDto);
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
