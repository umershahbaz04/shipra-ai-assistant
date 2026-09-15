using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.CommonUseCase.Response;
using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CommonFeatures.Query.GetAllProductOptionLookupQuery;
public class GetAllProductOptionLookupQuery : IRequest<ServiceResultDTO>
{
}
public class GetAllProductOptionLookupQueryHandler : RequestHandlerBase<GetAllProductOptionLookupQuery, ServiceResultDTO>
{
  private readonly ICommonLookupRepository _commonLookupRepository;

  public GetAllProductOptionLookupQueryHandler(ICommonLookupRepository commonLookupRepository, IServiceProvider serviceProvider, ILogger<GetAllProductOptionLookupQuery> logger) : base(serviceProvider, logger)
  {
    _commonLookupRepository = commonLookupRepository;
  }

  protected async override Task<ServiceResultDTO> HandleRequest(GetAllProductOptionLookupQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var productOptionList = await _commonLookupRepository.GetAllProductOptionLookup();
      productOptionList!.Insert(0, new ProductOptionLookup { ProductOptionId = ApplicationConstants.DropDownPlaceHolderId, Name = ApplicationConstants.DropDownPlaceHolderName });
      if (productOptionList is not null)
      {
        var dto = productOptionList!.Select(x => new CommonLookupResponseModel
        {
          id = x.ProductOptionId,
          text = x.Name,
        }).ToList();

        serviceResult = new ServiceResultDTO(dto);
      }
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
