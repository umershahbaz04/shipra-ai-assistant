using System.Dynamic;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.WebhookEventFeature.Query.GetClientWebhookEventById;
public class GetClientWebhookEventByIdQuery : IRequest<ServiceResultDTO>
{
  public int ClientWebhookEventId { get; set; }
}
public class GetClientWebhookEventByIdQueryHandler : RequestHandlerBase<GetClientWebhookEventByIdQuery, ServiceResultDTO>
{
  private readonly IWebHookEventRepository _webHookEventRepository;

  public GetClientWebhookEventByIdQueryHandler(IWebHookEventRepository webHookEventRepository, IServiceProvider serviceProvider, ILogger<GetClientWebhookEventByIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _webHookEventRepository = webHookEventRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetClientWebhookEventByIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var oClientWebhookEndpoint = await _webHookEventRepository.GetClientWebhookEventById(request.ClientWebhookEventId);
      if (oClientWebhookEndpoint is null)
      {
        throw new EntityNotFoundException("Client Webhook Endpoint ", request.ClientWebhookEventId!);
      }
        
      serviceResult = new ServiceResultDTO(new
      {
        oClientWebhookEndpoint.ClientWebhookEventId,
        oClientWebhookEndpoint.WebhookEventLookupId,
        webhookUrl = oClientWebhookEndpoint.WebhookUrl,
        isActive = oClientWebhookEndpoint.IsActive,
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
