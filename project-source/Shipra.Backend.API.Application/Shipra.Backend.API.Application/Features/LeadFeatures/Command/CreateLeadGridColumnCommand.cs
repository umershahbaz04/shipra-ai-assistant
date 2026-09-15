using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.LeadAggregate;

namespace Shipra.Backend.API.Application.Features.LeadFeatures.Command;

public class CreateLeadGridColumnCommand : IRequest<ServiceResultDTO>
{
    public string? ColumnName { get; set; }
    /// <summary>Comma-separated LeadStatusId values for this tab</summary>
    public string? DashboardStatusIdValues { get; set; }
}

public class CreateLeadGridColumnCommandHandler : RequestHandlerBase<CreateLeadGridColumnCommand, ServiceResultDTO>
{
    private readonly ILeadRepository _leadRepository;

    public CreateLeadGridColumnCommandHandler(ILeadRepository leadRepository, IServiceProvider serviceProvider, ILogger<CreateLeadGridColumnCommandHandler> logger) : base(serviceProvider, logger)
    {
        _leadRepository = leadRepository;
    }

    protected override async Task<ServiceResultDTO> HandleRequest(CreateLeadGridColumnCommand request, CancellationToken cancellationToken)
    {
        var serviceResult = new ServiceResultDTO();
        try
        {
            await _leadRepository.CreateLeadGridColumn(
                request.ColumnName!,
                request.DashboardStatusIdValues,
                _currentUser.ClientId!,
                _currentUser.EmployeeId!);
            serviceResult.IsSuccess = true;
            return serviceResult;
        }
        catch (Exception ex)
        {
            serviceResult.CreateErrorResponse(ex);
            throw;
        }
    }
}
