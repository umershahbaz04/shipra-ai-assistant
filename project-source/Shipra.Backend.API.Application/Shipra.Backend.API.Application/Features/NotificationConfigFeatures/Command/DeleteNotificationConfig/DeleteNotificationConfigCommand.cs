using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.NotificationAggregate;

namespace Shipra.Backend.API.Application.Features.NotificationConfigFeatures.Command.DeleteNotificationConfig;
public class DeleteNotificationConfigCommand : IRequest<ServiceResultDTO>
{
  public string? NotificationConfigId { get; set; }
}
public class DeleteNotificationConfigCommandHandler : RequestHandlerBase<DeleteNotificationConfigCommand, ServiceResultDTO>
{
  private readonly INotificationRepository _notificationRepository;

  public DeleteNotificationConfigCommandHandler(INotificationRepository notificationRepository, IServiceProvider serviceProvider, ILogger<DeleteNotificationConfigCommandHandler> logger) : base(serviceProvider, logger)
  {
    _notificationRepository = notificationRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(DeleteNotificationConfigCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      NotificationConfigId notificationConfigId = new NotificationConfigId(new Guid(request.NotificationConfigId!));

      var obj = await _notificationRepository.GetNotificationConfigById(notificationConfigId, _currentUser.ClientId!);
      if (obj is null)
      {
        throw new EntityNotFoundException("Configuration", request.NotificationConfigId!);
      }

      var res = await _notificationRepository.DeleteNotificationConfig(obj);
      if (res)
      {
        var notificationChnl = await _notificationRepository.GetNotificationChannelBySCId(obj.NotificationChannelId.GetValueOrDefault());
        if (notificationChnl is not null)
        {
          var ncs = await _notificationRepository.DeleteNotificationChannel(notificationChnl);

        }
      }
      serviceResult = new ServiceResultDTO(new BaseResponseDto
      {
        Data = true,
        Message = "Action perform successfully"
      });

      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class DeleteNotificationConfigCommandValidator : AbstractValidator<DeleteNotificationConfigCommand>
{
  public DeleteNotificationConfigCommandValidator()
  {
    RuleFor(v => v.NotificationConfigId).NotNull().NotEmpty().Must(GuidHelper.Validator).WithMessage(GuidHelper.GuidMessage);
  }
}
