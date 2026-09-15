using MediatR;
using Nancy;
using Nancy.Json;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Core.ActivityLogAggregate;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Grpc;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.NotificationAggregate;
using Shipra.Backend.API.Core.WebhookEventAggregate;
using Shipra.Backend.API.SharedKernel.Interfaces;

namespace Shipra.Backend.API.Application.MediatorNotification;
public class RequestActivityLog : INotification
{
  public string? Request { get; set; }
  public string? Response { get; set; }
  public string? EventName { get; set; }
  public string? ClientId { get; set; }
  public string? EmployeeId { get; set; }
  public bool? IsWebhookEvent { get; set; } = false;
  public DateTime? CreateOn { get; set; }
}
public class RequestNotificationHandler : INotificationHandler<RequestActivityLog>
{
  private readonly IWebHookEventRepository _webHookEventRepository;
  private readonly INotificationRepository _notificationRepository;
  private readonly IActivityLogRepository _activityLogRepository;
  private readonly IGrpcClientService _grpcClientService;

  public RequestNotificationHandler(ISharedStripeRepository adminSharedRepository, IWebHookEventRepository webHookEventRepository, INotificationRepository notificationRepository, IActivityLogRepository activityLogRepository, IGrpcClientService grpcClientService)
  {
    _webHookEventRepository = webHookEventRepository;
    _notificationRepository = notificationRepository;
    _activityLogRepository = activityLogRepository;
    _grpcClientService = grpcClientService;
  }

  public async Task Handle(RequestActivityLog request, CancellationToken cancellationToken)
  {
    #region activity log 
    await _activityLogRepository.Create(ActivityLog.Create(
        request.Request,
        request.Response,
        request.EventName,
        new ClientId(new Guid(request.ClientId!)),
        new EmployeeId(new Guid(request.EmployeeId!)),
        request.CreateOn));
    #endregion

    #region send notification message
    if (!string.IsNullOrEmpty(request.ClientId))
    {
      var json = JsonConvert.SerializeObject(request);
      var clientId = new ClientId(new Guid(request.ClientId));

      // Check aggregate contain
      var contain = ApplicationConstantConfig.EventCommandPairs
          .Where(x => x.EventName == request.EventName)
          .FirstOrDefault();

      if (contain is not null)
      {
        // gRPC Notification in a separate thread
        await Task.Run(async () =>
        {
          try
          {
            NotificationEvent? notificationEvent = await _notificationRepository.GetNotificationEventsByName(contain.EventName);

            if (notificationEvent is not null)
            {
              var notificationConfig = await _notificationRepository.GetNotificationConfigByClientId(clientId);
              if (notificationConfig.Count > 0)
              {
                var config = notificationConfig.FirstOrDefault(x => x.NotificationEventId == notificationEvent.NotificationEventId);
                if (config is not null)
                {
                  try
                  {
                    await _grpcClientService.SendMessageAsync(request.ClientId, json);
                  }
                  catch (Exception)
                  {
                    // log or ignore
                  }
                }
              }
            }
          }
          catch (Exception)
          {
            // log or ignore
          }
        });

        // Webhook Notification in a separate thread
        if (request.IsWebhookEvent.GetValueOrDefault())
        {
          _ = Task.Run(async () =>
          {
            try
            {
              WebhookEventLookup? oWebhookEventLookup = await _webHookEventRepository.GetWebHookEventsByName(contain.EventName);

              if (oWebhookEventLookup is not null)
              {
                var wrbhookConfig = await _webHookEventRepository.GetAllClientWebhookEvents(clientId);
                if (wrbhookConfig.Count > 0)
                {
                  var config = wrbhookConfig.FirstOrDefault(x => x.WebhookEventLookupId == oWebhookEventLookup.WebhookEventLookupId);
                  if (config is not null)
                  {
                    try
                    {
                      await _grpcClientService.SendWebhookMessageAsync(request.ClientId, json);
                    }
                    catch (Exception)
                    {
                      // log or ignore
                    }
                  }
                }
              }
            }
            catch (Exception)
            {
              // log or ignore
            }
          });
        }
      }

      #region on status update create followup
      if (request.EventName == ApplicationEvents.onordertrackingstatus)
      {
        _ = Task.Run(async () =>
        {
          try
          {
            await _grpcClientService.SendFollowupMessageAsync(request.ClientId, json);
          }
          catch (Exception)
          {
            // log or ignore
          }
        });
      }
      #endregion
      //#region on status update create followup
      //if (request.EventName == ApplicationEvents.onordercreated) //other geenric events we will handle here 
      //{
      //  //_ = Task.Run(async () =>
      //  //{
      //  try
      //  {
      //    await _grpcClientService.SendGeneralMessageAsync(request.ClientId, json);
      //  }
      //  catch (Exception ex)
      //  {
      //    _ = ex;
      //    // log or ignore
      //  }
      //  //});
      //}
      //#endregion
    }
    #endregion
  }
  #region old

  //public async Task Handle(RequestActivityLog request, CancellationToken cancellationToken)
  //{
  //  #region activity log 
  //  await _activityLogRepository.Create(ActivityLog.Create(request.Request, request.Response, request.EventName, new ClientId(new Guid(request.ClientId!)), new EmployeeId(new Guid(request.EmployeeId!)), request.CreateOn));
  //  #endregion

  //  // Start the notification sending process without waiting for it to complete
  //  //#region notification area
  //  ////  _ = Task.Run(async () =>
  //  ////{
  //  #region send notification message
  //  if (!string.IsNullOrEmpty(request.ClientId))
  //  {
  //    var json = JsonConvert.SerializeObject(request);
  //    var clientId = new ClientId(new Guid(request.ClientId));

  //    // Check aggregate contain
  //    var contain = ApplicationConstantConfig.EventCommandPairs.Where(x => x.EventName == request.EventName).FirstOrDefault();

  //    if (contain is not null)
  //    {
  //      NotificationEvent? notificationEvent = await _notificationRepository.GetNotificationEventsByName(contain.EventName);

  //      if (notificationEvent is not null)
  //      {
  //        var notificationConfig = await _notificationRepository.GetNotificationConfigByClientId(clientId);
  //        if (notificationConfig.Count > 0)
  //        {
  //          var config = notificationConfig.FirstOrDefault(x => x.NotificationEventId == notificationEvent.NotificationEventId);
  //          if (config is not null)
  //          {
  //            try
  //            {
  //              var msgResponse = await _grpcClientService.SendMessageAsync(request.ClientId, json);
  //            }
  //            catch (Exception)
  //            {

  //            }
  //          }
  //        }

  //      }

  //      //if also event configured then 
  //      if (request.IsWebhookEvent.GetValueOrDefault())
  //      {
  //        WebhookEventLookup? oWebhookEventLookup = await _webHookEventRepository.GetWebHookEventsByName(contain.EventName);

  //        if (oWebhookEventLookup is not null)
  //        {
  //          var wrbhookConfig = await _webHookEventRepository.GetAllClientWebhookEvents(clientId);
  //          if (wrbhookConfig.Count > 0)
  //          {
  //            var config = wrbhookConfig.FirstOrDefault(x => x.WebhookEventLookupId == oWebhookEventLookup.WebhookEventLookupId);
  //            if (config is not null)
  //            {
  //              try
  //              {
  //                var msgResponseWebhook = await _grpcClientService.SendWebhookMessageAsync(request.ClientId, json);

  //              }
  //              catch (Exception)
  //              {
  //              }

  //            }
  //          }
  //        }
  //      }
  //    }

  //    #region on status update create followup
  //    if (request.EventName == ApplicationEvents.onordertrackingstatus)
  //    {
  //      var msgResponseFollowUp = await _grpcClientService.SendFollowupMessageAsync(request.ClientId, json);
  //    }
  //    #endregion
  //  }
  //  #endregion
  //  ////});
  //  //#endregion
  //  #region webhook event

  //  #endregion
  //}

  #endregion
}
