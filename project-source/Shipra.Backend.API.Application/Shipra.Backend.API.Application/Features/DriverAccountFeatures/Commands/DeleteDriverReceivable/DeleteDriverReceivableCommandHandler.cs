using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DriverAccountFeatures.Commands.DeleteDriverReceivable;

public class DeleteDriverReceivableCommandHandler : RequestHandlerBase<DeleteDriverReceivableCommand, ServiceResultDTOWithTypeModel<BaseResponseDto>>
{
  private readonly IDriverAccountRepository _driverAccount;

  public DeleteDriverReceivableCommandHandler(IDriverAccountRepository driverAccount,IServiceProvider serviceProvider, ILogger<DeleteDriverReceivableCommandHandler> logger) : base(serviceProvider, logger)
  {
    _driverAccount = driverAccount;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<BaseResponseDto>> HandleRequest(DeleteDriverReceivableCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTOWithTypeModel<BaseResponseDto> serviceResult = new ServiceResultDTOWithTypeModel<BaseResponseDto>();
    try
    {
      if (!GuidHelper.Validator(request.DriverReceivableId!))
      {
        throw new InvalidIdTypeException($"{nameof(request.DriverReceivableId)} {request!.DriverReceivableId}");
      }

      var driverRecivable = await _driverAccount.GetDriverReceivableById(new DriverReceivableId(new Guid(request.DriverReceivableId!)));
      if (driverRecivable is null)
      {
        throw new EntityNotFoundException("DriverReceivable", request.DriverReceivableId!);
      }

      serviceResult.IsSuccess = await _driverAccount.DeleteDriverReceivable(driverRecivable);

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
