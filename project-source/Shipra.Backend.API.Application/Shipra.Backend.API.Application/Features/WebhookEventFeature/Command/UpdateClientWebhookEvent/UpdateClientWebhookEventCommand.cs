using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.WebhookEventFeature.Command.UpdateClientWebhookEvent;
public class UpdateClientWebhookEventCommand : IRequest<ServiceResultDTO>
{
  public int ClientWebhookEventId { get; set; }
  public int WebhookEventLookupId { get; set; }
  public string? WebhookUrl { get; set; }

}
public class UpdateClientWebhookEventCommandHandler : RequestHandlerBase<UpdateClientWebhookEventCommand, ServiceResultDTO>
{
  private readonly IWebHookEventRepository _webHookEventRepository;

  public UpdateClientWebhookEventCommandHandler(IWebHookEventRepository webHookEventRepository, IServiceProvider serviceProvider, ILogger<UpdateClientWebhookEventCommandHandler> logger) : base(serviceProvider, logger)
  {
    _webHookEventRepository = webHookEventRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdateClientWebhookEventCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var oClientWebhookEvent = await _webHookEventRepository.GetClientWebhookEventById(request.ClientWebhookEventId);
      if (oClientWebhookEvent is null)
      {
        throw new EntityNotFoundException("Client Webhook Endpoint ", request.ClientWebhookEventId!);
      }
      oClientWebhookEvent!.Update(request.WebhookEventLookupId, request.WebhookUrl, _currentUser.EmployeeId!);
      await _webHookEventRepository.UpdateClientWebhookEvent(oClientWebhookEvent);

      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
