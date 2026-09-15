using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.WebhookEventFeature.Command.DeleteWebhookEvent;
public class DeleteWebhookEventCommand : IRequest<ServiceResultDTO>
{
  public long ClientWebhookEventId { get; set; }
}
public class DeleteWebhookEventCommandHandler : RequestHandlerBase<DeleteWebhookEventCommand, ServiceResultDTO>
{
  private readonly IWebHookEventRepository _webHookEventRepository;

  public DeleteWebhookEventCommandHandler(IWebHookEventRepository webHookEventRepository,IServiceProvider serviceProvider, ILogger<DeleteWebhookEventCommandHandler> logger) : base(serviceProvider, logger)
  {
    _webHookEventRepository = webHookEventRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(DeleteWebhookEventCommand request, CancellationToken cancellationToken)
  { 
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var oClientWebhookEvent = await _webHookEventRepository.GetClientWebhookEventById(request.ClientWebhookEventId);
      if (oClientWebhookEvent is null)
      {
        throw new EntityNotFoundException("Client Webhook Endpoint ", request.ClientWebhookEventId!);
      }

      var isDeleted = await _webHookEventRepository.DeleteWebhookEvent(oClientWebhookEvent);
      if (isDeleted.GetValueOrDefault())
      {
        serviceResult = new ServiceResultDTO(new BaseResponseDto { Message = "Deleted successfully" });
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
