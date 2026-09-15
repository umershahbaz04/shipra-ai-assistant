using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.NotificationAggregate;

namespace Shipra.Backend.API.Application.Features.NotificationConfigFeatures.Query.GetNotificationTypes;
public class GetNotificationTypesQuery : IRequest<ServiceResultDTO>
{
}
public class GetNotificationTypesQueryHandler : RequestHandlerBase<GetNotificationTypesQuery, ServiceResultDTO>
{
  private readonly INotificationRepository _notificationRepository;

  public GetNotificationTypesQueryHandler(INotificationRepository notificationRepository, IServiceProvider serviceProvider, ILogger<GetNotificationTypesQueryHandler> logger) : base(serviceProvider, logger)
  {
    _notificationRepository = notificationRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetNotificationTypesQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {

      var oList = await _notificationRepository.GetNotificationTypes();

      var obj = NotificationType.AddDefault();
      oList?.Add(obj);
      var data = oList!.Select(x => new
      {
        x.NotificationTypeId,
        x.TypeName
      }).OrderBy(x => x.NotificationTypeId).ToList();

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
