using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.WebhookEventFeature.Query.GetAllClientWebhookEvent;
public class GetAllWebhookEventLogByClientQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; } 
}
public class GetAllWebhookEventLogByClientHandler : RequestHandlerBase<GetAllWebhookEventLogByClientQuery, ServiceResultDTO>
{
  private readonly IWebHookEventRepository _webHookEventRepository;

  public GetAllWebhookEventLogByClientHandler(IWebHookEventRepository webHookEventRepository, IServiceProvider serviceProvider, ILogger<GetAllWebhookEventLogByClientHandler> logger) : base(serviceProvider, logger)
  {
    _webHookEventRepository = webHookEventRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllWebhookEventLogByClientQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var filter = request.FilterModel!;
      var clientId = _currentUser.ClientId!.Value.ToString();
      dynamic data = await _webHookEventRepository.GetAllWebhookEventLogByClient(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!, clientId);

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
