using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.LeadAggregate;
using Shipra.Backend.API.Core.Enum;

namespace Shipra.Backend.API.Application.Features.LeadFeatures.Command;

public class UpdateOrderIdsCommand : IRequest<ServiceResultDTO>
{
    public Guid LeadId { get; set; }
    public long? OrderDraftId { get; set; }
    public Guid? OrderId { get; set; }
}

public class UpdateOrderIdsCommandHandler : RequestHandlerBase<UpdateOrderIdsCommand, ServiceResultDTO>
{
    private readonly ILeadRepository _leadRepository;

    public UpdateOrderIdsCommandHandler(ILeadRepository leadRepository, IServiceProvider serviceProvider, ILogger<UpdateOrderIdsCommandHandler> logger) : base(serviceProvider, logger)
    {
        _leadRepository = leadRepository;
    }

    protected override async Task<ServiceResultDTO> HandleRequest(UpdateOrderIdsCommand request, CancellationToken cancellationToken)
    {
        ServiceResultDTO serviceResult = new ServiceResultDTO();

        var lead = await _leadRepository.GetLeadById(new LeadId(request.LeadId));
        if (lead == null)
        {
            throw new EntityNotFoundException("Lead", request.LeadId.ToString());
        }

        if (request.OrderDraftId.HasValue)
        {
            lead.OrderDraftId = request.OrderDraftId.Value;
        }

        if (request.OrderId.HasValue)
        {
            lead.OrderId = request.OrderId.Value;
            lead.OrderDraftId = null;
            lead.LeadStatusId = (int)EnumLeadStatusLookup.Completed;
        }

        lead.UpdatedBy = _currentUser.EmployeeId;
        lead.UpdatedOn = DateTime.UtcNow;

        await _leadRepository.UpdateLead(lead);

        return serviceResult;
    }
}

