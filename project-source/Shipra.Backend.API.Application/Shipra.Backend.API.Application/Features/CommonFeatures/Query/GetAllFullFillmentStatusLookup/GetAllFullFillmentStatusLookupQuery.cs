using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.CommonUseCase.Response;
using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CommonFeatures.Query.GetAllFullFillmentStatusLookupQuery;
public class GetAllFullFillmentStatusLookupQuery : IRequest<ServiceResultDTOWithTypeModel<List<CommonLookupResponseModel>>>
{
}
public class GetAllFullFillmentStatusLookupQueryHandler : RequestHandlerBase<GetAllFullFillmentStatusLookupQuery, ServiceResultDTOWithTypeModel<List<CommonLookupResponseModel>>>
{
  private readonly ICommonLookupRepository _commonLookupRepository;
  public GetAllFullFillmentStatusLookupQueryHandler(ICommonLookupRepository commonLookupRepository, IServiceProvider serviceProvider, ILogger<GetAllFullFillmentStatusLookupQuery> logger) : base(serviceProvider, logger)
  {
    _commonLookupRepository = commonLookupRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<List<CommonLookupResponseModel>>> HandleRequest(GetAllFullFillmentStatusLookupQuery request, CancellationToken cancellationToken)
  {
    var response = new ServiceResultDTOWithTypeModel<List<CommonLookupResponseModel>>();
    try
    {
      var expense = await _commonLookupRepository.GetAllFullFillmentStatusLookup();
      var obj = FullFillmentStatusLookup.AddDefault();
      expense?.Add(obj);
      var data = expense!.Select(x => new CommonLookupResponseModel
      {
        id = x.FullFillmentStatusId,
        text = x.FullFillmentStatus
      }).OrderBy(x =>x.id).ToList();
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
