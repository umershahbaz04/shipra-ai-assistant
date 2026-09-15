using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CommonFeatures.Query.GetAllSCFolderLookup;
public class GetAllSCFolderLookupQueryHandler : RequestHandlerBase<GetAllSCFolderLookupForSelectionQuery, ServiceResultDTO>
{
  private readonly ICommonLookupRepository _commonLookupRepository;

  public GetAllSCFolderLookupQueryHandler(ICommonLookupRepository commonLookupRepository, IServiceProvider serviceProvider, ILogger<GetAllSCFolderLookupQueryHandler> logger) : base(serviceProvider, logger)
  {
    _commonLookupRepository = commonLookupRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllSCFolderLookupForSelectionQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var oSCFolderLookupData = await _commonLookupRepository.GetAllSCFolderLookup();
      if (oSCFolderLookupData is not null)
      {
        var selectedSCAttributes = oSCFolderLookupData!.Select(x => new { x.ScfolderLookupId, x.ScfolderName });
        serviceResult = new ServiceResultDTO(selectedSCAttributes);
        serviceResult.CreateSuccessResponse(System.Net.HttpStatusCode.OK);
        return serviceResult!;
      }
      else
      {
        serviceResult.CreateErrorResponse(new Exception(NotificationConstants.Error));
        return serviceResult;
      }
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
