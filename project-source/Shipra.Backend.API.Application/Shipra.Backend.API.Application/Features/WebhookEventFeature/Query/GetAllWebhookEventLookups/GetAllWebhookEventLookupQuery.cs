using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.TaxAggregate;
using Shipra.Backend.API.Core.WebhookEventAggregate;

namespace Shipra.Backend.API.Application.Features.WebhookEventFeature.Query.GetAllWebhookEventLookups;
public class GetAllWebhookEventLookupQuery : IRequest<ServiceResultDTO>
{
}
public class GetAllWebhookEventLookupQueryHandler : RequestHandlerBase<GetAllWebhookEventLookupQuery, ServiceResultDTO>
{
  private readonly IWebHookEventRepository _webHookEventRepository;

  public GetAllWebhookEventLookupQueryHandler(IWebHookEventRepository webHookEventRepository, IServiceProvider serviceProvider, ILogger<GetAllWebhookEventLookupQueryHandler> logger) : base(serviceProvider, logger)
  {
    _webHookEventRepository = webHookEventRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllWebhookEventLookupQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var list = await _webHookEventRepository.GetAllWebhookEventLookups();
      //list.Add(WebhookEventLookup.AddDefault());
      var newList = list.OrderBy(x => x.WebhookEventLookupId).ToList();
      serviceResult = new ServiceResultDTO(newList);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
