using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.SMSProcessFeature.Command.DeleteSMSActivate;
public class DeleteSMSActivateCommandHandler : RequestHandlerBase<DeleteSMSActivateCommand, ServiceResultDTO>
{
  private readonly ISMSProcessRepository _smsProcessRepository;

  public DeleteSMSActivateCommandHandler(ISMSProcessRepository smsProcessRepository, IServiceProvider serviceProvider, ILogger<DeleteSMSActivateCommandHandler> logger) : base(serviceProvider, logger)
  {
    _smsProcessRepository = smsProcessRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(DeleteSMSActivateCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var smsActive = await _smsProcessRepository.GetSMSActivateById(request.SMSActivateId, _currentUser.ClientId!);
      if (smsActive == null)
      {
        throw new EntityNotFoundException("SMSActive", request.SMSActivateId);
      }
      //delete flag set
      smsActive.DeleteSMSActivate(_currentUser.EmployeeId);
      var updatedData = await _smsProcessRepository.DeleteSMSActivate(smsActive);
      serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = updatedData, Message = NotificationConstants.Success });
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}

