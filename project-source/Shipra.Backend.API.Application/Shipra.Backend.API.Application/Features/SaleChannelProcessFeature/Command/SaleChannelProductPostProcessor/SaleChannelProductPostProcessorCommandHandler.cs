using System.Net;
using Autofac.Features.Indexed;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.SaleChannelProcessFeature.Command.SaleChannelProductPostProcessor;

public class SaleChannelProductPostProcessorCommandHandler : RequestHandlerBase<SaleChannelProductPostProcessorCommand, ServiceResultDTO>
{
  private readonly IClientRepository _clientRepository;
  private readonly ISaleChannelConfigRepository _saleChannelConfigRepository;
  private readonly IIndex<int, ISaleChannelProductPostProcessorService> _processorServices;

  public SaleChannelProductPostProcessorCommandHandler(IClientRepository clientRepository, ISaleChannelConfigRepository saleChannelConfigRepository, IIndex<int, ISaleChannelProductPostProcessorService> processorServices, IServiceProvider serviceProvider, ILogger<SaleChannelProductPostProcessorCommandHandler> logger) : base(serviceProvider, logger)
  {
    _clientRepository = clientRepository;
    _saleChannelConfigRepository = saleChannelConfigRepository;
    _processorServices = processorServices;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(SaleChannelProductPostProcessorCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      if (request.SaleChannelLookupId == null ||
          !_processorServices.TryGetValue(request.SaleChannelLookupId.Value, out var processor))
      {
        serviceResult.CreateErrorResponse(HttpStatusCode.BadRequest);
        return serviceResult;
      }

      return await processor.PostProcessProductsAsync(request, _currentUser.ClientId!, _currentUser.EmployeeId, cancellationToken);
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
