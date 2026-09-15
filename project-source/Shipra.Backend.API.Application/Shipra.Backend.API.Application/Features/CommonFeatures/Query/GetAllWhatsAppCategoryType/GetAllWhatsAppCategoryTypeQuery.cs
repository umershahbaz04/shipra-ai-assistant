using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.CommonUseCase.Response;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CommonFeatures.Query.GetAllWhatsAppCategoryLookupQuery;
public class GetAllWhatsAppCategoryTypeQuery : IRequest<ServiceResultDTOWithTypeModel<List<CommonLookupResponseModel>>>
{
}
public class GetAllWhatsAppCategoryTypeQueryHandler : RequestHandlerBase<GetAllWhatsAppCategoryTypeQuery, ServiceResultDTOWithTypeModel<List<CommonLookupResponseModel>>>
{
  private readonly ICommonLookupRepository _commonLookupRepository;
  public GetAllWhatsAppCategoryTypeQueryHandler(ICommonLookupRepository commonLookupRepository, IServiceProvider serviceProvider, ILogger<GetAllWhatsAppCategoryTypeQuery> logger) : base(serviceProvider, logger)
  {
    _commonLookupRepository = commonLookupRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<List<CommonLookupResponseModel>>> HandleRequest(GetAllWhatsAppCategoryTypeQuery request, CancellationToken cancellationToken)
  {
    var response = new ServiceResultDTOWithTypeModel<List<CommonLookupResponseModel>>();
    try
    {
      var expense = await _commonLookupRepository.GetAllWhatsAppCategoryLookup();
      var data = expense!.Select(x => new CommonLookupResponseModel
      {
        id = x.WhatsAppCategoryTypeId,
        text = x.Name,
      }).ToList();
      response = new ServiceResultDTOWithTypeModel<List<CommonLookupResponseModel>>(data);
      response.CreateSuccessResponse();
      return response;
    }
    catch (Exception ex)
    {
      response.CreateErrorResponse(ex);
      throw;
    }
  }
}
