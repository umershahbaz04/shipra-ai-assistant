using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.MediatorNotification;

namespace Shipra.Backend.API.Application.Features.CommonFeature.Command.HandleNotificationAndEvent;
public class HandleNotificationAndEventCommand : IRequest<ServiceResultDTO>
{
  public string? OrderNos { get; set; }
  public string? ClientId { get; set; }
}
public class HandleNotificationAndEventCommandHandler : RequestHandlerBase<HandleNotificationAndEventCommand, ServiceResultDTO>
{
  private readonly IMediator _mediator;

  public HandleNotificationAndEventCommandHandler(IMediator mediator, IServiceProvider serviceProvider, ILogger<HandleNotificationAndEventCommandHandler> logger) : base(serviceProvider, logger)
  {
    _mediator = mediator;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(HandleNotificationAndEventCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      _currentUser.ClientIdStr = request.ClientId;
      _currentUser.EmployeeIdStr = request.ClientId;
      #region publish notification 
      await _mediator.Publish(new
              RequestActivityLog
      {
        Request = JsonConvert.SerializeObject(request),
        Response = JsonConvert.SerializeObject(serviceResult),
        EventName = ApplicationEvents.onordertrackingstatus, 
        IsWebhookEvent = true,
        ClientId = _currentUser.ClientIdStr,
        EmployeeId = _currentUser.EmployeeIdStr,
        CreateOn = DateTime.UtcNow
      });
      #endregion
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }

  }
}
