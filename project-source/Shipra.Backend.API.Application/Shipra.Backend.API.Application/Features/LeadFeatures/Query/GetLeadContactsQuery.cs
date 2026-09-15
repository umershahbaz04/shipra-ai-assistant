using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.LeadFeatures.Query;

public class GetLeadContactsQuery : IRequest<ServiceResultDTO>
{
    public int Start { get; set; }
    public int Length { get; set; }
    public string? Search { get; set; }
}

public class GetLeadContactsQueryHandler : RequestHandlerBase<GetLeadContactsQuery, ServiceResultDTO>
{
    private readonly ILeadRepository _leadRepository;

    public GetLeadContactsQueryHandler(ILeadRepository leadRepository, IServiceProvider serviceProvider, ILogger<GetLeadContactsQueryHandler> logger) : base(serviceProvider, logger)
    {
        _leadRepository = leadRepository;
    }

    protected override async Task<ServiceResultDTO> HandleRequest(GetLeadContactsQuery request, CancellationToken cancellationToken)
    {
        ServiceResultDTO serviceResult = new ServiceResultDTO();
        string clientId = _currentUser.ClientId!.Value.ToString();
        var result = await _leadRepository.GetLeadContacts(clientId, request.Start, request.Length, request.Search);
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
