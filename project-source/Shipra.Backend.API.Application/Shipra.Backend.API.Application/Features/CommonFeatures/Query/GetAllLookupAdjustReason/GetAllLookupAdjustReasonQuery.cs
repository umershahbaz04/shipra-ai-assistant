using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.CommonUseCase.Response;
using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CommonFeatures.Query.GetAllLookupAdjustReasonQuery;
public class GetAllLookupAdjustReasonQuery : IRequest<ServiceResultDTOWithTypeModel<List<CommonLookupResponseModel>>>
{
}
public class GetAllLookupAdjustReasonQueryHandler : RequestHandlerBase<GetAllLookupAdjustReasonQuery, ServiceResultDTOWithTypeModel<List<CommonLookupResponseModel>>>
{
  private readonly ICommonLookupRepository _commonLookupRepository;
  public GetAllLookupAdjustReasonQueryHandler(ICommonLookupRepository commonLookupRepository, IServiceProvider serviceProvider, ILogger<GetAllLookupAdjustReasonQuery> logger) : base(serviceProvider, logger)
  {
    _commonLookupRepository = commonLookupRepository;
  }

  protected override Task<ServiceResultDTOWithTypeModel<List<CommonLookupResponseModel>>> HandleRequest(GetAllLookupAdjustReasonQuery request, CancellationToken cancellationToken)
  {
    var response = new ServiceResultDTOWithTypeModel<List<CommonLookupResponseModel>>();
    try
    {
      var enumValues = Enum.GetValues(typeof(Shipra.Backend.API.Core.Enum.InventoryTransactionType))
                           .Cast<Shipra.Backend.API.Core.Enum.InventoryTransactionType>()
                           .Select(e => new CommonLookupResponseModel
                           {
                               id = (int)e,
                               text = e.ToString()
                           }).ToList();

      var defaultItem = new CommonLookupResponseModel
      {
          id = 0,
          text = "Select Reason"
      };
      enumValues.Insert(0, defaultItem);

      response = new ServiceResultDTOWithTypeModel<List<CommonLookupResponseModel>>(enumValues);
      response.CreateSuccessResponse();
      return Task.FromResult(response);
    }
    catch (Exception ex)
    {
      response.CreateErrorResponse(ex);
      throw;
    }
  }
}
