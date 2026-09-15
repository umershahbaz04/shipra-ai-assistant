using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CommonFeatures.Query.GetAllONGFTypeLookup;
public class GetAllONGFTypeLookupQuery : IRequest<ServiceResultDTO>
{
}
public class GetAllONGFTypeLookupQueryHandler : RequestHandlerBase<GetAllONGFTypeLookupQuery, ServiceResultDTO>
{
  private readonly ICommonLookupRepository _commonLookupRepository;

  public GetAllONGFTypeLookupQueryHandler(ICommonLookupRepository commonLookupRepository, IServiceProvider serviceProvider, ILogger<GetAllONGFTypeLookupQueryHandler> logger) : base(serviceProvider, logger)
  {
    _commonLookupRepository = commonLookupRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllONGFTypeLookupQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var data = await _commonLookupRepository.GetAllONGFTypeLookp();
      serviceResult = new ServiceResultDTO(data);
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
