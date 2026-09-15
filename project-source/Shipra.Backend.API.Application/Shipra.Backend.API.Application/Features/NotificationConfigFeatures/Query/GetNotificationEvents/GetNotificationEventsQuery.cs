using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.NotificationAggregate;

namespace Shipra.Backend.API.Application.Features.NotificationConfigFeatures.Query.GetNotificationEvents;
public class GetNotificationEventsQuery : IRequest<ServiceResultDTO>
{
}
public class GetNotificationEventsQueryHandler : RequestHandlerBase<GetNotificationEventsQuery, ServiceResultDTO>
{
  private readonly INotificationRepository _notificationRepository;

  public GetNotificationEventsQueryHandler(INotificationRepository notificationRepository, IServiceProvider serviceProvider, ILogger<GetNotificationEventsQueryHandler> logger) : base(serviceProvider, logger)
  {
    _notificationRepository = notificationRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetNotificationEventsQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {

      var oList = await _notificationRepository.GetNotificationEvents();

      var obj = NotificationEvent.AddDefault();
      oList?.Add(obj);
      var data = oList!.Select(x => new
      {
        x.NotificationEventId,
        x.EventName,
        x.EventKey

      }).OrderBy(x => x.NotificationEventId).ToList();

      serviceResult = new ServiceResultDTO(data);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
