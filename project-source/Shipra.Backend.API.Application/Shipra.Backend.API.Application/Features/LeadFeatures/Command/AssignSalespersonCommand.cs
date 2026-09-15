using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.LeadAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Application.Features.LeadFeatures.Command;

public class AssignSalespersonCommand : IRequest<ServiceResultDTO>
{
    public List<Guid> LeadIds { get; set; } = new List<Guid>();
    public Guid SalespersonId { get; set; }
}

public class AssignSalespersonCommandHandler : RequestHandlerBase<AssignSalespersonCommand, ServiceResultDTO>
{
    private readonly ILeadRepository _leadRepository;

    public AssignSalespersonCommandHandler(ILeadRepository leadRepository, IServiceProvider serviceProvider, ILogger<AssignSalespersonCommandHandler> logger) : base(serviceProvider, logger)
    {
        _leadRepository = leadRepository;
    }

    protected override async Task<ServiceResultDTO> HandleRequest(AssignSalespersonCommand request, CancellationToken cancellationToken)
    {
        ServiceResultDTO serviceResult = new ServiceResultDTO();

        foreach (var leadId in request.LeadIds)
        {
            var lead = await _leadRepository.GetLeadById(new LeadId(leadId));
            if (lead == null) continue;
            lead.AssignSalesperson(new EmployeeId(request.SalespersonId), _currentUser.EmployeeId);
            await _leadRepository.UpdateLead(lead);
        }

        serviceResult = new ServiceResultDTO("Salesperson assigned successfully.");
        return serviceResult;
    }
}
