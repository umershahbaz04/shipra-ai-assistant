using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.SaleChannelProcessFeature.Command.SaleChannelOrderPostProcessor;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Application.Services.Interfaces;

/// <summary>
/// Unified strategy interface for Sale Channel Order Post-Processors.
/// Each sale channel provides its own implementation, registered in DI keyed by
/// <see cref="Shipra.Backend.API.Core.Enum.EnumSaleChannelLookup"/> int value.
/// </summary>
public interface ISaleChannelOrderPostProcessorService
{
  Task<ServiceResultDTO> PostProcessOrdersAsync(
    SaleChannelOrderPostProcessorCommand request,
    ClientId clientId,
    EmployeeId? employeeId,
    CancellationToken cancellationToken);
}
