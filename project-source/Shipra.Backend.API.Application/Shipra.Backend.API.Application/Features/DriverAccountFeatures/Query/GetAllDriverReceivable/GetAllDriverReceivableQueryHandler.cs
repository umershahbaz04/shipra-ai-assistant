using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DriverAccountFeatures.Query.GetAllDriverReceivable;
public class GetAllDriverReceivableQueryHandler : RequestHandlerBase<GetAllDriverReceivableQuery, ServiceResultDTO>
{
  private readonly IDriverAccountRepository _driverAccount;

  public GetAllDriverReceivableQueryHandler(IDriverAccountRepository driverAccount, IServiceProvider serviceProvider, ILogger<GetAllDriverReceivableQueryHandler> logger) : base(serviceProvider, logger)
  {
    _driverAccount = driverAccount;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllDriverReceivableQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request.FilterModel!;
      var oDriverReceiveable = await _driverAccount.GetAllIDriverReceivables(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!,_currentUser.ClientIdStr!, request.DriverIds);
      if (oDriverReceiveable is not null)
      {
        serviceResult = new ServiceResultDTO(oDriverReceiveable);
        serviceResult.CreateSuccessResponse();
      }
      else
      {
        serviceResult.CreateErrorResponse(System.Net.HttpStatusCode.NotFound);
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

