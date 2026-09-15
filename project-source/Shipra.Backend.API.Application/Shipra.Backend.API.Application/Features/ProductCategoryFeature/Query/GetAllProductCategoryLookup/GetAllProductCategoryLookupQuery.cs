using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Application.Features.ProductCategoryFeature.Query.GetAllProductCategoryLookup;
public class GetAllProductCategoryLookupQuery : IRequest<ServiceResultDTO>
{
}
public class GetAllProductCategoryLookupQueryHandler : RequestHandlerBase<GetAllProductCategoryLookupQuery, ServiceResultDTO>
{
  private readonly IProductCategoryRepository _productCategoryRepository;

  public GetAllProductCategoryLookupQueryHandler(IProductCategoryRepository productCategoryRepository, IServiceProvider serviceProvider, ILogger<GetAllProductCategoryLookupQueryHandler> logger) : base(serviceProvider, logger)
  {
    _productCategoryRepository = productCategoryRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllProductCategoryLookupQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var productCategoryList = await _productCategoryRepository.GetAllProductCategoryLookupByClientId(_currentUser.ClientId!);
      productCategoryList!.Insert(0, new ProductCategory { ProductCategoryId = ApplicationConstants.DropDownPlaceHolderId, CategoryName = ApplicationConstants.DropDownPlaceHolderName });
      if (productCategoryList is not null)
      {
        var dto = productCategoryList!.Select(x => new { ProductCategoryId = x.ProductCategoryId, CategoryName = x.CategoryName }).ToList();
        serviceResult = new ServiceResultDTO(dto);
        serviceResult.CreateSuccessResponse();
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
