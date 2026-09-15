using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.DeliveryNoteAggregate;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DriverAccountFeatures.Commands.UpdateDriverReceivable;

public class UpdateDriverReceivableCommandHandler : RequestHandlerBase<UpdateDriverReceivableCommand, ServiceResultDTOWithTypeModel<BaseResponseDto>>
{
  private readonly IDriverAccountRepository _driverAccount;

  public UpdateDriverReceivableCommandHandler(IDriverAccountRepository driverAccount, IServiceProvider serviceProvider, ILogger<UpdateDriverReceivableCommandHandler> logger) : base(serviceProvider, logger)
  {
    _driverAccount = driverAccount;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<BaseResponseDto>> HandleRequest(UpdateDriverReceivableCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTOWithTypeModel<BaseResponseDto> serviceResult = new ServiceResultDTOWithTypeModel<BaseResponseDto>();
    try
    {
      if (!GuidHelper.Validator(request.DriverId!))
      {
        throw new InvalidIdTypeException($"{nameof(request.DriverId)} {request!.DriverId!}");
      }

      if (!GuidHelper.Validator(request.DriverReceivableId!))
      {
        throw new InvalidIdTypeException($"{nameof(request.DriverReceivableId)} {request!.DriverReceivableId}");
      }

      var driverRecivable = await _driverAccount.GetDriverReceivableById(new DriverReceivableId(new Guid(request.DriverReceivableId!)));
      if (driverRecivable is null)
      {
        throw new EntityNotFoundException("DriverReceivable", request.DriverReceivableId!);
      }
      driverRecivable!.UpdateDriverRecivable(new DriverId(new Guid(request!.DriverId!)), request.ReceiveDate, request.Expense, request.Cash, request.Total, _currentUser.EmployeeId!, request.Active,
        new DeliveryNoteId(new Guid(request!.DeliveryNoteId!)));

      serviceResult.IsSuccess = await _driverAccount.UpdateDriverReceivable(driverRecivable);

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
