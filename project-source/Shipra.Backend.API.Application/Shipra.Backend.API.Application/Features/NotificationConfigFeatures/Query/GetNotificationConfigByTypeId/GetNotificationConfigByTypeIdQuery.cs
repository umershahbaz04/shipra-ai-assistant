using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.NotificationUseCase;
using Shipra.Backend.API.Application.MediatorNotification;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.NotificationConfigFeatures.Query.GetNotificationConfigByTypeId;
public class GetNotificationConfigByTypeIdQuery : IRequest<ServiceResultDTO>
{
  public int NotificationTypeId { get; set; }
}
public class GetNotificationConfigByTypeIdQueryHandler : RequestHandlerBase<GetNotificationConfigByTypeIdQuery, ServiceResultDTO>
{
  private readonly IMediator _mediator;
  private readonly INotificationRepository _notificationRepository;

  public GetNotificationConfigByTypeIdQueryHandler(IMediator mediator, INotificationRepository notificationRepository, IServiceProvider serviceProvider, ILogger<GetNotificationConfigByTypeIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _mediator = mediator;
    _notificationRepository = notificationRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetNotificationConfigByTypeIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var oList = await _notificationRepository.GetNotificationConfigByTypeId(request.NotificationTypeId, _currentUser.ClientId);
       
      serviceResult = new ServiceResultDTO(oList);

      //#region publish notification 
      //await _mediator.publish(new
      //        requestactivitylog
      //{
      //  request = jsonconvert.serializeobject(new
      //  {
      //    ordernos = "15700164",
      //    request.notificationtypeid
      //  }),
      //  response = jsonconvert.serializeobject(serviceresult),
      //  aggregate = "onordertrackingstatus",
      //  clientid = _currentuser.clientidstr,
      //  employeeid = _currentuser.employeeidstr,
      //  createon = datetime.utcnow
      //});
      //#endregion


      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class GetNotificationConfigByTypeIdQueryValidator : AbstractValidator<GetNotificationConfigByTypeIdQuery>
{
  public GetNotificationConfigByTypeIdQueryValidator()
  {
    RuleFor(v => v.NotificationTypeId).NotNull().NotEmpty().GreaterThan(0);
  }
}
