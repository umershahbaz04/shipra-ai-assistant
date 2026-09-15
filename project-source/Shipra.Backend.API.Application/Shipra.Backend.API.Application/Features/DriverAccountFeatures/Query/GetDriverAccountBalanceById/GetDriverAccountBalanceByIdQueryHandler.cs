using Microsoft.Extensions.Logging;
using NPOI.HSSF.Record;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DriverAccountFeatures.Query.GetDriverAccountBalanceById;

public class GetDriverAccountBalanceByIdQueryHandler : RequestHandlerBase<GetDriverAccountBalanceByIdQuery, ServiceResultDTO>
{
  private readonly IDriverAccountRepository _driverAccountRepository;
  private readonly IDriverRepository _driverRepository;

  public GetDriverAccountBalanceByIdQueryHandler(IDriverAccountRepository driverAccountRepository, IDriverRepository driverRepository, IServiceProvider serviceProvider, ILogger<GetDriverAccountBalanceByIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _driverAccountRepository = driverAccountRepository;
    _driverRepository = driverRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetDriverAccountBalanceByIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var oDriver = await _driverRepository.GetDriverByEmployeeId(_currentUser.EmployeeId!);
      if (oDriver is not null)
      {
        var oDriverAccountBalance = await _driverAccountRepository.GetDriverAccountBalanceById(oDriver.DriverId!.Value.ToString()!, _currentUser.ClientIdStr!);
        if (oDriverAccountBalance is not null)
        {
          serviceResult = new ServiceResultDTO(oDriverAccountBalance);
          serviceResult.CreateSuccessResponse(System.Net.HttpStatusCode.OK);
          return serviceResult;
        }
        else
        {
          throw new ShipraApplicationException(System.Net.HttpStatusCode.ExpectationFailed, "Driver Account Balance not found");
        }
      }
      else
      {
        throw new ShipraApplicationException(System.Net.HttpStatusCode.ExpectationFailed, "Driver not found");
      }
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
