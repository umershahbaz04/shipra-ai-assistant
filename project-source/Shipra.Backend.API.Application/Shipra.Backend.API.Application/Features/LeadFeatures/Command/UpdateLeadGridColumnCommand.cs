using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.LeadFeatures.Command;

public class UpdateLeadGridColumnRequest
{
    public int LeadGridColumnId { get; set; }
    public string? ColumnName { get; set; }
    public string? DashboardStatusValue { get; set; }
}

public class UpdateLeadGridColumnCommand : IRequest<ServiceResultDTO>
{
    public List<UpdateLeadGridColumnRequest>? List { get; set; }
}

public class UpdateLeadGridColumnCommandHandler : RequestHandlerBase<UpdateLeadGridColumnCommand, ServiceResultDTO>
{
    private readonly ILeadRepository _leadRepository;

    public UpdateLeadGridColumnCommandHandler(ILeadRepository leadRepository, IServiceProvider serviceProvider, ILogger<UpdateLeadGridColumnCommandHandler> logger) : base(serviceProvider, logger)
    {
        _leadRepository = leadRepository;
    }

    protected override async Task<ServiceResultDTO> HandleRequest(UpdateLeadGridColumnCommand request, CancellationToken cancellationToken)
    {
        ServiceResultDTO serviceResult = new ServiceResultDTO();
        try
        {
            if (request.List == null || !request.List.Any())
            {
                serviceResult.CreateError("Validation", new[] { "List must contain at least one item." });
                return serviceResult;
            }

            foreach (var item in request.List)
            {
                await _leadRepository.UpdateLeadGridColumn(
                    item.LeadGridColumnId,
                    item.ColumnName?.Trim().ToUpper()!,
                    item.DashboardStatusValue!,
                    _currentUser.ClientId!,
                    _currentUser.EmployeeId!);
            }

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
