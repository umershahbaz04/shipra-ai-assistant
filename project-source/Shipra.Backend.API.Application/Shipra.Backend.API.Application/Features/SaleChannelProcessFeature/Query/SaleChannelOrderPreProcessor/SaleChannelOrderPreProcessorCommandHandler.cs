using System.Net;
using Autofac.Features.Indexed;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.SaleChannelProcessFeature.Query.SaleChannelOrderPreProcessor;

public class SaleChannelOrderPreProcessorCommandHandler : RequestHandlerBase<SaleChannelOrderPreProcessorCommand, ServiceResultDTO>
{
  private readonly IClientRepository _clientRepository;
  private readonly ISaleChannelConfigRepository _SaleChannelConfigRepository;
  private readonly IStoreRepository _storeRepository;

  // Autofac keyed index: resolves the correct strategy by EnumSaleChannelLookup int value.
  // No if/else needed — adding a new sale channel only requires a new keyed DI registration.
  private readonly IIndex<int, ISaleChannelOrderPreProcessorService> _processorServices;

  public SaleChannelOrderPreProcessorCommandHandler(IClientRepository clientRepository,ISaleChannelConfigRepository SaleChannelConfigRepository,IStoreRepository storeRepository,IIndex<int, ISaleChannelOrderPreProcessorService> processorServices,IServiceProvider serviceProvider,ILogger<SaleChannelOrderPreProcessorCommandHandler> logger) : base(serviceProvider, logger)
  {
    _clientRepository = clientRepository;
    _SaleChannelConfigRepository = SaleChannelConfigRepository;
    _storeRepository = storeRepository;
    _processorServices = processorServices;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(SaleChannelOrderPreProcessorCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var oSaleChannelConfig = await _SaleChannelConfigRepository.GetSaleChannelConfigById(request.SaleChannelConfigId, _currentUser.ClientId!);
      if (oSaleChannelConfig == null)
      {
        throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "SaleChannelConfig not found");
      }

      var oStore = await _storeRepository.GetStoreById(request.StoreId, _currentUser.ClientId!);

      #region reset date
      int? regionTimeMinut = await _clientRepository.GetClientRegionMinutes(_currentUser.ClientIdStr!);
      if (regionTimeMinut != null)
      {
        request.CreatedFrom = request.CreatedFrom.AddMinutes(-regionTimeMinut.Value);
        request.CreatedTo = request.CreatedTo.AddMinutes(-regionTimeMinut.Value);
      }
      #endregion

      // Resolve the correct sale channel processor strategy by lookup id.
      // Store is required for all current channel processors.
      if (oSaleChannelConfig.SaleChannelLookupId == null ||
          !_processorServices.TryGetValue(oSaleChannelConfig.SaleChannelLookupId.Value, out var processor))
      {
        serviceResult.CreateErrorResponse(HttpStatusCode.BadRequest);
        return serviceResult;
      }

      if (oStore == null)
      {
        throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "Store not found");
      }

      return await processor.PreProcessOrdersAsync(request, oSaleChannelConfig, oStore, _currentUser.ClientId!, cancellationToken);
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
