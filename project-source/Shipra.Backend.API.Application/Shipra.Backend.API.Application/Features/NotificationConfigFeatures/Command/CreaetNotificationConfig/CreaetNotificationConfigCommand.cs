using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.NotificationUseCase;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.NotificationAggregate;

namespace Shipra.Backend.API.Application.Features.NotificationFeatures.Command.CreaetNotificationConfig;
public class CreaetNotificationConfigCommand : IRequest<ServiceResultDTO>
{
  public List<CreateUpdateNotificationDto>? list { get; set; } = new();
}
public class CreaetNotificationConfigCommandHandler : RequestHandlerBase<CreaetNotificationConfigCommand, ServiceResultDTO>
{
  private readonly INotificationRepository _notificationRepository;

  public CreaetNotificationConfigCommandHandler(INotificationRepository notificationRepository, IServiceProvider serviceProvider, ILogger<CreaetNotificationConfigCommandHandler> logger) : base(serviceProvider, logger)
  {
    _notificationRepository = notificationRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreaetNotificationConfigCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      foreach (var item in request.list!)
      {
        NotificationChannel notificationChannel = NotificationChannel.Create(item.NotificationTypeId, item.ServiceTypeId);
        bool dt = await _notificationRepository.CreateNotificationChannel(notificationChannel);

        NotificationConfig notificationConfig = NotificationConfig.Create(item.Text, notificationChannel.NotificationChannelId, _currentUser.ClientId!, item.NotificationEventId, item.Config, _currentUser.EmployeeId!);
        var res = await _notificationRepository.CreaetNotificationConfig(notificationConfig);

        serviceResult = new ServiceResultDTO(new BaseResponseDto
        {
          Data = res,
          Message = "Action perform successfully"
        });
      }


      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class CreaetNotificationConfigCommandValidator : AbstractValidator<CreaetNotificationConfigCommand>
{
  public CreaetNotificationConfigCommandValidator()
  {
    RuleFor(x => x.list).Must(x => x != null).WithMessage("Notification list must contain at least one item.");
    RuleForEach(model => model.list).SetValidator(model => new CreaetUpdateNotificationConfigOptionValidator());
  }
}
public class CreaetUpdateNotificationConfigOptionValidator : AbstractValidator<CreateUpdateNotificationDto>
{
  public CreaetUpdateNotificationConfigOptionValidator()
  {
    RuleFor(v => v.Text).NotNull().NotEmpty();
    RuleFor(v => v.ServiceTypeId).NotNull().NotEmpty().GreaterThan(0);
    RuleFor(v => v.NotificationTypeId).NotNull().NotEmpty().GreaterThan(0);
    RuleFor(v => v.NotificationEventId).NotNull().NotEmpty().GreaterThan(0); 
  }
}
