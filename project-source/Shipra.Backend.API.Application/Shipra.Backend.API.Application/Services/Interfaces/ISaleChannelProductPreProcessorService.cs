using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.SaleChannelProcessFeature.Query.SaleChannelProductPreProcessor;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.SaleChannelConfigAggregate;

namespace Shipra.Backend.API.Application.Services.Interfaces;

/// <summary>
/// Unified strategy interface for Sale Channel Product Pre-Processors.
/// Each sale channel provides its own implementation, registered in DI keyed by
/// <see cref="Shipra.Backend.API.Core.Enum.EnumSaleChannelLookup"/> int value.
/// </summary>
public interface ISaleChannelProductPreProcessorService
{
  Task<ServiceResultDTO> PreProcessProductsAsync(
    SaleChannelProductPreProcessorCommand request,
    SaleChannelConfig oSaleChannelConfig,
    ClientId clientId,
    CancellationToken cancellationToken);
}
