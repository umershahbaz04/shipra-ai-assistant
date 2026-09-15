using System.Net;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DriverAccountFeatures.Query.GetCODClearedByDriver;
public class GetCODClearedByDriverQueryHandler : RequestHandlerBase<GetCODClearedByDriverQuery, ServiceResultDTO>
{
  private readonly IDriverAccountRepository _driverAccount;
  private readonly IDriverRepository _driverRepository;

  public GetCODClearedByDriverQueryHandler(IDriverRepository driverRepository, IDriverAccountRepository driverAccount, IServiceProvider serviceProvider, ILogger<GetCODClearedByDriverQueryHandler> logger) : base(serviceProvider, logger)
  {
    _driverAccount = driverAccount;
    _driverRepository = driverRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetCODClearedByDriverQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var oDriver = await _driverRepository.GetDriverByEmployeeId(_currentUser.EmployeeId!);
      if (oDriver is not null)
      {
        dynamic data = await _driverAccount.GetAllCODClearedByDriverId(oDriver.DriverId!.Value.ToString(),_currentUser.ClientIdStr!);
        serviceResult = new ServiceResultDTO(data);
        return serviceResult;
      }
      else
      {
        throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "Driver not found");
      }
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
