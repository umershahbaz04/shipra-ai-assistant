using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.DeliveryNoteAggregate;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DriverAccountFeatures.Commands.CreateDriverReceivable;

public class CreateDriverReceivableCommandHandler : RequestHandlerBase<CreateDriverReceivableCommand, ServiceResultDTO>
{
  private readonly IDriverAccountRepository _driverAccount;

  public CreateDriverReceivableCommandHandler(IDriverAccountRepository driverAccount, IServiceProvider serviceProvider, ILogger<CreateDriverReceivableCommandHandler> logger) : base(serviceProvider, logger)
  {
    _driverAccount = driverAccount; 
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateDriverReceivableCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var response = await _driverAccount.CreateDriverReceivable(DriverReceivable.CreateDriverRecivable(new DriverId(new Guid(request!.DriverId!)), request.Expense, request.Cash, request.Total, _currentUser.EmployeeId!,
        new DeliveryNoteId(new Guid(request!.DeliveryNoteId!))));
      var result = new BaseResponseDto()
      {
        Data = response,
        Message = response.Message
      };
      return new ServiceResultDTO(result);
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
