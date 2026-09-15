using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.CommonUseCase.Response;
using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CommonFeatures.Query.GetAllAddressTypeLookupQuery;
public class GetAllAddressTypeLookupQuery : IRequest<ServiceResultDTOWithTypeModel<List<CommonLookupResponseModel>>>
{

}
public class GetAllAddressTypeLookupQueryHandler : RequestHandlerBase<GetAllAddressTypeLookupQuery, ServiceResultDTOWithTypeModel<List<CommonLookupResponseModel>>>
{
  private readonly ICommonLookupRepository _commonLookupRepository;

  public GetAllAddressTypeLookupQueryHandler(ICommonLookupRepository commonLookupRepository, IServiceProvider serviceProvider, ILogger<GetAllAddressTypeLookupQuery> logger) : base(serviceProvider, logger)
  {
    _commonLookupRepository = commonLookupRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<List<CommonLookupResponseModel>>> HandleRequest(GetAllAddressTypeLookupQuery request, CancellationToken cancellationToken)
  {
    var response = new ServiceResultDTOWithTypeModel<List<CommonLookupResponseModel>>();
    try
    {
      var expense = await _commonLookupRepository.GetAllAddressTypeLookup();
      var obj = AddressTypeLookup.AddDefault();
      expense?.Add(obj);
      var data = expense!.Select(x => new CommonLookupResponseModel()
      {
        id = x.AddressTypeId,
        text = x.AddressTypeName
      }).OrderBy(x => x.id).ToList();

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
