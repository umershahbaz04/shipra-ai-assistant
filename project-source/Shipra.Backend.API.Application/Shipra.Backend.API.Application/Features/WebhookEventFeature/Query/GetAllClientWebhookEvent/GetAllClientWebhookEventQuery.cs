using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.WebhookEventFeature.Query.GetAllClientWebhookEvent;
public class GetAllClientWebhookEventQuery : IRequest<ServiceResultDTO>
{
}
public class GetAllClientWebhookEventQueryHandler : RequestHandlerBase<GetAllClientWebhookEventQuery, ServiceResultDTO>
{
  private readonly IWebHookEventRepository _webHookEventRepository;

  public GetAllClientWebhookEventQueryHandler(IWebHookEventRepository webHookEventRepository,IServiceProvider serviceProvider, ILogger<GetAllClientWebhookEventQueryHandler> logger) : base(serviceProvider, logger)
  {
    _webHookEventRepository = webHookEventRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllClientWebhookEventQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    { 
      var clientId = _currentUser.ClientId!.Value.ToString();
      dynamic data = await _webHookEventRepository.GetAllClientWebhookEvents(clientId);

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
