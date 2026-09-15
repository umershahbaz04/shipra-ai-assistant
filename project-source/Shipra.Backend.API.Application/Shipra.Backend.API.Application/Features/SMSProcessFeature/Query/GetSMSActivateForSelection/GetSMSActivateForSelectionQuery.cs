using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.SMSProcessFeature.Query.GetSMSActivateForSelection;
public class GetSMSActivateForSelectionQuery : IRequest<ServiceResultDTO>
{
}
public class GetSMSActivateForSelectionQueryHandler : RequestHandlerBase<GetSMSActivateForSelectionQuery, ServiceResultDTO>
{
  private readonly ISMSProcessRepository _sMSProcessRepository;

  public GetSMSActivateForSelectionQueryHandler(ISMSProcessRepository sMSProcessRepository,IServiceProvider serviceProvider, ILogger<GetSMSActivateForSelectionQueryHandler> logger) : base(serviceProvider, logger)
  {
    _sMSProcessRepository = sMSProcessRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetSMSActivateForSelectionQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var oList = await _sMSProcessRepository.GetSMSActivateForSelection(_currentUser.ClientIdStr!);
      serviceResult = new ServiceResultDTO(oList!);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
