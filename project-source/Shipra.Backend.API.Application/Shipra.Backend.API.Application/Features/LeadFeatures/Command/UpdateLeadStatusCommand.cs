using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.LeadAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Application.Features.LeadFeatures.Command;

public class UpdateLeadStatusCommand : IRequest<ServiceResultDTO>
{
    public Guid LeadId { get; set; }
    public int LeadStatusId { get; set; }
}

public class UpdateLeadStatusCommandHandler : RequestHandlerBase<UpdateLeadStatusCommand, ServiceResultDTO>
{
    private readonly ILeadRepository _leadRepository;

    public UpdateLeadStatusCommandHandler(ILeadRepository leadRepository, IServiceProvider serviceProvider, ILogger<UpdateLeadStatusCommandHandler> logger) : base(serviceProvider, logger)
    {
        _leadRepository = leadRepository;
    }

    protected override async Task<ServiceResultDTO> HandleRequest(UpdateLeadStatusCommand request, CancellationToken cancellationToken)
    {
        ServiceResultDTO serviceResult = new ServiceResultDTO();

        var lead = await _leadRepository.GetLeadById(new LeadId(request.LeadId));
        if (lead == null)
        {
            throw new EntityNotFoundException("Lead", request.LeadId.ToString());
        }

        lead.UpdateLeadStatus(request.LeadStatusId, _currentUser.EmployeeId);
        await _leadRepository.UpdateLead(lead);

        return serviceResult;
    }
}
