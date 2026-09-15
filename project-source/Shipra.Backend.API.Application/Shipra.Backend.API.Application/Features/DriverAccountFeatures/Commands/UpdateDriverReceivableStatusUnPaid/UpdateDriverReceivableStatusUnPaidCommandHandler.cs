using System.Net;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DriverAccountFeatures.Commands.UpdateDriverReceivableStatusUnPaid;

public class UpdateDriverReceivableStatusUnPaidCommandHandler : RequestHandlerBase<UpdateDriverReceivableStatusUnPaidCommand, ServiceResultDTO>
{
  private readonly IDriverAccountRepository _driverAccount;

  public UpdateDriverReceivableStatusUnPaidCommandHandler(IDriverAccountRepository driverAccount, IServiceProvider serviceProvider, ILogger<UpdateDriverReceivableStatusUnPaidCommandHandler> logger) : base(serviceProvider, logger)
  {
    _driverAccount = driverAccount;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdateDriverReceivableStatusUnPaidCommand request, CancellationToken cancellationToken)
  {
    var response = new ServiceResultDTO();

    try
    {
      var driverRecivable = await _driverAccount.GetDriverReceivableById(new DriverReceivableId(new Guid(request.DriverReceivableId!)));
      if (driverRecivable is null)
      {
        throw new EntityNotFoundException("DriverReceivable", request.DriverReceivableId!);
      }
      driverRecivable.UpdateDriverRecivablePaidStatus((int)EnumDriverPaidStatus.UnPaid, _currentUser.EmployeeId!);
      var oDriverRecivable = await _driverAccount.UpdateDriverReceivable(driverRecivable);
      var result = new BaseResponseDto()
      {
        Data = oDriverRecivable,
        Message = NotificationConstants.UpdateSuccess
      };
      response.CreateSuccessResponse(HttpStatusCode.OK);
      return response;
    }
    catch (Exception ex)
    {
      response.CreateErrorResponse(ex);
      throw;
    }
  }
}
