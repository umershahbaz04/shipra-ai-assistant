using System.Net;
using Autofac.Features.Indexed;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.MediatorNotification;
using Shipra.Backend.API.Application.Services.Interfaces;

namespace Shipra.Backend.API.Application.Features.SaleChannelProcessFeature.Command.SaleChannelOrderPostProcessor;

public class SaleChannelOrderPostProcessorCommandHandler : RequestHandlerBase<SaleChannelOrderPostProcessorCommand, ServiceResultDTO>
{
  private readonly IMediator _mediator;

  // Autofac keyed index: resolves the correct strategy by EnumSaleChannelLookup int value.
  // No if/else needed — adding a new sale channel only requires a new keyed DI registration.
  private readonly IIndex<int, ISaleChannelOrderPostProcessorService> _processorServices;

  public SaleChannelOrderPostProcessorCommandHandler(IMediator mediator, IIndex<int, ISaleChannelOrderPostProcessorService> processorServices, IServiceProvider serviceProvider, ILogger<SaleChannelOrderPostProcessorCommandHandler> logger) : base(serviceProvider, logger)
  {
    _mediator = mediator;
    _processorServices = processorServices;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(SaleChannelOrderPostProcessorCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      // Resolve the correct sale channel processor strategy by lookup id.
      if (request.SaleChannelLookupId == null ||
          !_processorServices.TryGetValue(request.SaleChannelLookupId.Value, out var processor))
      {
        serviceResult.CreateErrorResponse(HttpStatusCode.BadRequest);
        return serviceResult;
      }

      serviceResult = await processor.PostProcessOrdersAsync(request, _currentUser.ClientId!, _currentUser.EmployeeId, cancellationToken);

      #region publish notification
      await _mediator.Publish(new RequestActivityLog
      {
        Request = JsonConvert.SerializeObject(request),
        Response = JsonConvert.SerializeObject(serviceResult),
        EventName = "oncreateOrder",
        ClientId = _currentUser.ClientIdStr,
        EmployeeId = _currentUser.EmployeeIdStr,
        CreateOn = DateTime.UtcNow
      });
      #endregion

      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
