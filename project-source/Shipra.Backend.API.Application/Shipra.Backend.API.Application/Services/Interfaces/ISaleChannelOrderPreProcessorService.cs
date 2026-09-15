using System.Threading;
using System.Threading.Tasks;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.SaleChannelProcessFeature.Query.SaleChannelOrderPreProcessor;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.SaleChannelConfigAggregate;
using Shipra.Backend.API.Core.StoresAggregate;

namespace Shipra.Backend.API.Application.Services.Interfaces;

/// <summary>
/// Unified strategy interface for Sale Channel Order Pre-Processors.
/// Each sale channel (Shopify, WooCommerce, Amazon, etc.) provides its own
/// implementation, registered in DI keyed by <see cref="Shipra.Backend.API.Core.Enum.EnumSaleChannelLookup"/> int value.
/// Adding a new sale channel only requires a new implementation + one keyed DI registration —
/// no handler or controller changes are needed.
/// </summary>
public interface ISaleChannelOrderPreProcessorService
{
  Task<ServiceResultDTO> PreProcessOrdersAsync(
    SaleChannelOrderPreProcessorCommand request,
    SaleChannelConfig oSaleChannelConfig,
    Store? oStore,
    ClientId clientId,
    CancellationToken cancellationToken);
}
