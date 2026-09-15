using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;

using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.LeadFeatures.Query;

public class GetAllLeadsQuery : IRequest<ServiceResultDTO>
{
    public int Start { get; set; }
    public int Length { get; set; }
    public string? Search { get; set; }
    public int SortCol { get; set; }
    public string? SortDir { get; set; }
    public string? LeadStatusIds { get; set; }
    public string? SalespersonIds { get; set; }
    public string? AssignmentFilter { get; set; }
    
    public string? CountryId { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
}

public class GetAllLeadsQueryHandler : RequestHandlerBase<GetAllLeadsQuery, ServiceResultDTO>
{
    private readonly ILeadRepository _leadRepository;

    public GetAllLeadsQueryHandler(ILeadRepository leadRepository, IServiceProvider serviceProvider, ILogger<GetAllLeadsQueryHandler> logger) : base(serviceProvider, logger)
    {
        _leadRepository = leadRepository;
    }

    protected override async Task<ServiceResultDTO> HandleRequest(GetAllLeadsQuery request, CancellationToken cancellationToken)
    {
        ServiceResultDTO serviceResult = new ServiceResultDTO();
        string clientId = _currentUser.ClientId!.Value.ToString();
        var result = await _leadRepository.GetAllLeads(clientId, request.Start, request.Length, request.Search, request.SortCol, request.SortDir, request.LeadStatusIds, request.SalespersonIds, request.AssignmentFilter, request.CountryId, request.StartDate, request.EndDate);
        var resultList = result as IEnumerable<dynamic>;
        int totalCount = resultList != null && resultList.Any() ? (int)resultList.First().TotalCount : 0;
        serviceResult = new ServiceResultDTO(new { 
            draw = 0,
            recordsTotal = totalCount,
            recordsFiltered = totalCount,
            list = result
        });
        return serviceResult;
    }
}
