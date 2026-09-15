using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.LeadFeatures.Query;

public class GetAllClientLeadStatusForSelectionQuery : IRequest<ServiceResultDTO>
{
}

public class GetAllClientLeadStatusForSelectionQueryHandler : RequestHandlerBase<GetAllClientLeadStatusForSelectionQuery, ServiceResultDTO>
{
    private readonly ILeadRepository _leadRepository;

    public GetAllClientLeadStatusForSelectionQueryHandler(ILeadRepository leadRepository, IServiceProvider serviceProvider, ILogger<GetAllClientLeadStatusForSelectionQueryHandler> logger) : base(serviceProvider, logger)
    {
        _leadRepository = leadRepository;
    }

    protected override async Task<ServiceResultDTO> HandleRequest(GetAllClientLeadStatusForSelectionQuery request, CancellationToken cancellationToken)
    {
        ServiceResultDTO serviceResult = new ServiceResultDTO();
        try
        {
            await _leadRepository.SeedDefaultLeadStatuses(_currentUser.ClientId!, _currentUser.EmployeeId!);
            var result = await _leadRepository.GetAllClientLeadStatusForSelection(_currentUser.ClientId!);
            serviceResult = new ServiceResultDTO(result);
            return serviceResult;
        }
        catch (Exception ex)
        {
            serviceResult.CreateErrorResponse(ex);
            throw;
        }
    }
}
