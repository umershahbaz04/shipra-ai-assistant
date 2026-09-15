using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.LeadFeatures.Query;

public class GetLeadTabsCountConfigQuery : IRequest<ServiceResultDTO>
{
}

public class GetLeadTabsCountConfigQueryHandler : RequestHandlerBase<GetLeadTabsCountConfigQuery, ServiceResultDTO>
{
    private readonly ILeadRepository _leadRepository;

    public GetLeadTabsCountConfigQueryHandler(ILeadRepository leadRepository, IServiceProvider serviceProvider, ILogger<GetLeadTabsCountConfigQueryHandler> logger) : base(serviceProvider, logger)
    {
        _leadRepository = leadRepository;
    }

    protected override async Task<ServiceResultDTO> HandleRequest(GetLeadTabsCountConfigQuery request, CancellationToken cancellationToken)
    {
        ServiceResultDTO serviceResult = new ServiceResultDTO();
        try
        {
            var config = await _leadRepository.GetAllLeadGridClientSettingForDashboard(_currentUser.ClientIdStr!);
            serviceResult = new ServiceResultDTO(config);
            return serviceResult;
        }
        catch (Exception ex)
        {
            serviceResult.CreateErrorResponse(ex);
            throw;
        }
    }
}
