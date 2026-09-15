using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.NotificationUseCase;
using Shipra.Backend.API.Application.Features.NotificationFeatures.Command.CreaetNotificationConfig;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.NotificationAggregate;

namespace Shipra.Backend.API.Application.Features.NotificationConfigFeatures.Command.UpdateNotificationConfig;
public class UpdateNotificationConfigCommand : IRequest<ServiceResultDTO>
{
  public List<CreateUpdateNotificationDto>? list { get; set; } = new();
}
public class UpdateNotificationConfigCommandHandler : RequestHandlerBase<UpdateNotificationConfigCommand, ServiceResultDTO>
{
  private readonly INotificationRepository _notificationRepository;

  public UpdateNotificationConfigCommandHandler(INotificationRepository notificationRepository, IServiceProvider serviceProvider, ILogger<UpdateNotificationConfigCommandHandler> logger) : base(serviceProvider, logger)
  {
    _notificationRepository = notificationRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdateNotificationConfigCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      foreach (var item in request.list!)
      {
        //if id then update
        if (!string.IsNullOrEmpty(item.NotificationConfigId))
        { 
          NotificationConfigId notificationConfigId = new NotificationConfigId(new Guid(item.NotificationConfigId));
          var target = await _notificationRepository.GetNotificationConfigById(notificationConfigId, _currentUser.ClientId!);
          if (target is not null)
          {
            var notificationChannel = await _notificationRepository.GetNotificationChannelBySCId(target.NotificationChannelId.GetValueOrDefault());

            int? notificationChannelId = target.NotificationChannelId;
            if (notificationChannel is not null)
            {
              notificationChannel.Update(item.NotificationTypeId, item.ServiceTypeId);
              await _notificationRepository.UpdateNotificationChannel(notificationChannel);
              notificationChannelId = notificationChannel.NotificationChannelId;
            }

            target.Update(item.Text, notificationChannelId, _currentUser.ClientId!, item.NotificationEventId, item.Config, _currentUser.EmployeeId!);
            await _notificationRepository.UpdateNotificationConfig(target);
          };
        }
        else
        {
          NotificationChannel notificationChannel = NotificationChannel.Create(item.NotificationTypeId, item.ServiceTypeId);
          bool dt = await _notificationRepository.CreateNotificationChannel(notificationChannel);

          NotificationConfig notificationConfig = NotificationConfig.Create(item.Text, notificationChannel.NotificationChannelId, _currentUser.ClientId!, item.NotificationEventId, item.Config, _currentUser.EmployeeId!);
          await _notificationRepository.CreaetNotificationConfig(notificationConfig);
        }

        serviceResult = new ServiceResultDTO(new BaseResponseDto
        {
          Data = true,
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
public class UpdateNotificationConfigCommandValidator : AbstractValidator<UpdateNotificationConfigCommand>
{
  public UpdateNotificationConfigCommandValidator()
  {
    RuleFor(x => x.list).Must(x => x != null).WithMessage("Notification list must contain at least one item.");
    RuleForEach(model => model.list).SetValidator(model => new CreaetUpdateNotificationConfigOptionValidator());
  }
}
