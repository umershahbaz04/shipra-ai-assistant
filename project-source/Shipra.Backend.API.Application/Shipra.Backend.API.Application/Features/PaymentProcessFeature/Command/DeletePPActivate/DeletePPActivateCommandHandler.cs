using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.PaymentProcessFeature.Command.DeletePPActivate;
public class DeletePPActivateCommandHandler : RequestHandlerBase<DeletePPActivateCommand, ServiceResultDTO>
{
  private readonly IPaymentProcessRepository _paymentProcessRepository;

  public DeletePPActivateCommandHandler(IPaymentProcessRepository paymentProcessRepository, IServiceProvider serviceProvider, ILogger<DeletePPActivateCommandHandler> logger) : base(serviceProvider, logger)
  {
    _paymentProcessRepository = paymentProcessRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(DeletePPActivateCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var oPPActive = await _paymentProcessRepository.GetPPActivateByPPActivateId(request.PpactivateId, _currentUser.ClientId!);
      if (oPPActive == null)
      {
        throw new EntityNotFoundException("PPActive", request.PpactivateId);
      }
      //delete flag set
      oPPActive.DeletePpactivate(_currentUser.EmployeeId);
      var isDeleted = await _paymentProcessRepository.DeletePPActivate(oPPActive);
      serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = isDeleted, Message = NotificationConstants.Success });
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}

