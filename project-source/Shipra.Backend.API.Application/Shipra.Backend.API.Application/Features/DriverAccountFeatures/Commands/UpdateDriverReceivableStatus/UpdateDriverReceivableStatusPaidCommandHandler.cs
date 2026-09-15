using System.Net;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.DeliveryNoteAggregate;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.DriverAccountFeatures.Commands.UpdateDriverReceivableStatus;

public class UpdateDriverReceivableStatusPaidCommandHandler : RequestHandlerBase<UpdateDriverReceivableStatusPaidCommand, ServiceResultDTO>
{
  private readonly IDriverAccountRepository _driverAccount;
  private readonly IOrderTrackingHistoryRepository _orderTrackingHistoryRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly IDeliveryNoteRepository _deliveryNoteRepository;
  public UpdateDriverReceivableStatusPaidCommandHandler(IDriverAccountRepository driverAccount, IOrderTrackingHistoryRepository orderTrackingHistoryRepository, IDeliveryNoteRepository deliveryNoteRepository, IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<UpdateDriverReceivableStatusPaidCommandHandler> logger) : base(serviceProvider, logger)
  {
    _orderTrackingHistoryRepository = orderTrackingHistoryRepository;
    _orderRepository = orderRepository;
    _driverAccount = driverAccount;
    _deliveryNoteRepository = deliveryNoteRepository;

  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdateDriverReceivableStatusPaidCommand request, CancellationToken cancellationToken)
  {
    var response = new ServiceResultDTO();

    try
    {
      var driverRecivable = await _driverAccount.GetDriverReceivableById(new DriverReceivableId(new Guid(request.DriverReceivableId!)));
      if (driverRecivable is null)
      {
        throw new EntityNotFoundException("DriverReceivable", request.DriverReceivableId!);
      }

      driverRecivable.UpdateDriverRecivablePaidStatus((int)EnumDriverPaidStatus.Paid, _currentUser.EmployeeId!);
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
