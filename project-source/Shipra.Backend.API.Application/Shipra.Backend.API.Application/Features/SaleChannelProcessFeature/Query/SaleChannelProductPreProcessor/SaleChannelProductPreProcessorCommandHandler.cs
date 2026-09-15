using System.Net;
using Autofac.Features.Indexed;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.SaleChannelProcessFeature.Query.SaleChannelProductPreProcessor;

public class SaleChannelProductPreProcessorCommandHandler : RequestHandlerBase<SaleChannelProductPreProcessorCommand, ServiceResultDTO>
{
  private readonly IClientRepository _clientRepository;
  private readonly ISaleChannelConfigRepository _SaleChannelConfigRepository;

  // Autofac keyed index: resolves the correct strategy by EnumSaleChannelLookup int value.
  // No if/else needed — adding a new sale channel only requires a new keyed DI registration.
  private readonly IIndex<int, ISaleChannelProductPreProcessorService> _processorServices;

  public SaleChannelProductPreProcessorCommandHandler(IClientRepository clientRepository,ISaleChannelConfigRepository SaleChannelConfigRepository,IIndex<int, ISaleChannelProductPreProcessorService> processorServices,IServiceProvider serviceProvider,ILogger<SaleChannelProductPreProcessorCommandHandler> logger) : base(serviceProvider, logger)
  {
    _clientRepository = clientRepository;
    _SaleChannelConfigRepository = SaleChannelConfigRepository;
    _processorServices = processorServices;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(SaleChannelProductPreProcessorCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var oSaleChannelConfig = await _SaleChannelConfigRepository.GetSaleChannelConfigById(request.SaleChannelConfigId, _currentUser.ClientId!);
      if (oSaleChannelConfig == null)
      {
        throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "SaleChannelConfig not found");
      }

      #region reset date
      int? regionTimeMinut = await _clientRepository.GetClientRegionMinutes(_currentUser.ClientIdStr!);
      if (regionTimeMinut != null)
      {
        request.CreatedFrom = request.CreatedFrom.AddMinutes(-regionTimeMinut.Value);
        request.CreatedTo = request.CreatedTo.AddMinutes(-regionTimeMinut.Value);
      }
      #endregion

      // Resolve the correct sale channel processor strategy by lookup id.
      if (oSaleChannelConfig.SaleChannelLookupId == null ||
          !_processorServices.TryGetValue(oSaleChannelConfig.SaleChannelLookupId.Value, out var processor))
      {
        serviceResult.CreateErrorResponse(HttpStatusCode.BadRequest);
        return serviceResult;
      }

      return await processor.PreProcessProductsAsync(request, oSaleChannelConfig, _currentUser.ClientId!, cancellationToken);
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
