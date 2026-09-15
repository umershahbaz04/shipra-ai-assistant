using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.LeadFeatures.Query;

public class GetAllLeadStatusForSelectionQuery : IRequest<ServiceResultDTO>
{
}

public class GetAllLeadStatusForSelectionQueryHandler : RequestHandlerBase<GetAllLeadStatusForSelectionQuery, ServiceResultDTO>
{
    private readonly ILeadRepository _leadRepository;

    public GetAllLeadStatusForSelectionQueryHandler(ILeadRepository leadRepository, IServiceProvider serviceProvider, ILogger<GetAllLeadStatusForSelectionQueryHandler> logger) : base(serviceProvider, logger)
    {
        _leadRepository = leadRepository;
    }

    protected override async Task<ServiceResultDTO> HandleRequest(GetAllLeadStatusForSelectionQuery request, CancellationToken cancellationToken)
    {
        ServiceResultDTO serviceResult = new ServiceResultDTO();
        // Ensure per-client statuses are seeded before returning
        await _leadRepository.SeedDefaultLeadStatuses(_currentUser.ClientId!, _currentUser.EmployeeId!);
        // Return from ClientLeadStatusLookup (per-client), NOT the global LeadStatusLookups
        var result = await _leadRepository.GetAllClientLeadStatusForSelection(_currentUser.ClientId!);
        serviceResult = new ServiceResultDTO(result);
        return serviceResult;
    }
}
