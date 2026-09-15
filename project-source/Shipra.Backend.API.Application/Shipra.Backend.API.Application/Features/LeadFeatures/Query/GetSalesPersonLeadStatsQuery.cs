using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Application.Features.LeadFeatures.Query;

public class GetSalesPersonLeadStatsQuery : IRequest<ServiceResultDTO>
{
    public string? LeadStatusIds { get; set; }
    public string? SalespersonIds { get; set; }
    public string? Search { get; set; }
    public string? AssignmentFilter { get; set; }
    public string? CountryId { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
}

public class GetSalesPersonLeadStatsQueryHandler : RequestHandlerBase<GetSalesPersonLeadStatsQuery, ServiceResultDTO>
{
    private readonly ILeadRepository _leadRepository;

    public GetSalesPersonLeadStatsQueryHandler(ILeadRepository leadRepository, IServiceProvider serviceProvider, ILogger<GetSalesPersonLeadStatsQueryHandler> logger) : base(serviceProvider, logger)
    {
        _leadRepository = leadRepository;
    }

    protected override async Task<ServiceResultDTO> HandleRequest(GetSalesPersonLeadStatsQuery request, CancellationToken cancellationToken)
    {
        ServiceResultDTO serviceResult = new ServiceResultDTO();
        try
        {
            string clientId = _currentUser.ClientId!.Value.ToString();
            dynamic result = await _leadRepository.GetSalesPersonLeadStats(clientId, request.LeadStatusIds, request.SalespersonIds, request.Search, request.AssignmentFilter, request.CountryId, request.StartDate, request.EndDate);
            serviceResult = new ServiceResultDTO(result);
            serviceResult.CreateSuccessResponse();
            return serviceResult;
        }
        catch (Exception ex)
        {
            serviceResult.CreateErrorResponse(ex);
            throw;
        }
    }
}
