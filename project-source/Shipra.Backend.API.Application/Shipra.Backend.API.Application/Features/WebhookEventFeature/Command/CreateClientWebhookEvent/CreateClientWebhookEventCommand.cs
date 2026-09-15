using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.WebhookEventAggregate;

namespace Shipra.Backend.API.Application.Features.WebhookEventFeature.Command.CreateClientWebhookEvent;
public class CreateClientWebhookEventCommand : IRequest<ServiceResultDTO>
{
  public int? WebhookEventLookupId { get; set; }
  public string? WebhookUrl { get; set; }

}
public class CreateClientWebhookEventCommandValidator : RequestHandlerBase<CreateClientWebhookEventCommand, ServiceResultDTO>
{
  private readonly IWebHookEventRepository _webHookEventRepository;

  public CreateClientWebhookEventCommandValidator(IWebHookEventRepository webHookEventRepository, IServiceProvider serviceProvider, ILogger<CreateClientWebhookEventCommandValidator> logger) : base(serviceProvider, logger)
  {
    _webHookEventRepository = webHookEventRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateClientWebhookEventCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {

      ClientWebhookEvent clientWebhookEvent = ClientWebhookEvent.Create(request.WebhookEventLookupId.GetValueOrDefault(), request.WebhookUrl!, _currentUser.ClientId!, _currentUser.EmployeeId!);

      var IsClientWebhookEventExist = await _webHookEventRepository.IsClientWebhookEventExist(clientWebhookEvent);
      if (IsClientWebhookEventExist is not null)
      {
        serviceResult.CreateError("AlreadyExist", new string[] { $"{request.WebhookUrl} is already exist against selected event" });
        return serviceResult;
      }
      var isAdded = await _webHookEventRepository.CreateClientWebhookEvent(clientWebhookEvent);
      if (isAdded.GetValueOrDefault())
      { 
        serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = null, Message = "Webhook event added successfully" });
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
