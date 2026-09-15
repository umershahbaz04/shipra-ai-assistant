using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.LeadFeatures.Query;

public class GetAllClientLeadStatusQuery : IRequest<ServiceResultDTO>
{
    public FilterModelDTO? FilterModel { get; set; }
}

public class GetAllClientLeadStatusQueryHandler : RequestHandlerBase<GetAllClientLeadStatusQuery, ServiceResultDTO>
{
    private readonly ILeadRepository _leadRepository;

    public GetAllClientLeadStatusQueryHandler(ILeadRepository leadRepository, IServiceProvider serviceProvider, ILogger<GetAllClientLeadStatusQueryHandler> logger) : base(serviceProvider, logger)
    {
        _leadRepository = leadRepository;
    }

    protected override async Task<ServiceResultDTO> HandleRequest(GetAllClientLeadStatusQuery request, CancellationToken cancellationToken)
    {
        ServiceResultDTO serviceResult = new ServiceResultDTO();
        try
        {
            // Guard: if FilterModel is null use safe defaults
            var filter = request?.FilterModel ?? new FilterModelDTO
            {
                Start = 0,
                Length = 1000,
                Search = "",
                SortCol = 0,
                SortDir = "ASC"
            };

            // Auto-seed: if this client has no statuses yet, seed from LeadStatusLookup
            await _leadRepository.SeedDefaultLeadStatuses(_currentUser.ClientId!, _currentUser.EmployeeId!);

            var result = await _leadRepository.GetAllClientLeadStatus(
                _currentUser.ClientId!.Value.ToString(),
                filter.Start, filter.Length, filter.Search,
                filter.SortCol, filter.SortDir);
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
