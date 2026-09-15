using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.LeadAggregate;

namespace Shipra.Backend.API.Application.Features.LeadFeatures.Command;

public class UpdateLeadCommand : IRequest<ServiceResultDTO>
{
    public Guid LeadId { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ProductName { get; set; }
    public string? GoogleLocationLink { get; set; }
    public long? OrderDraftId { get; set; }
    public Guid? OrderId { get; set; }
}

public class UpdateLeadCommandHandler : RequestHandlerBase<UpdateLeadCommand, ServiceResultDTO>
{
    private readonly ILeadRepository _leadRepository;

    public UpdateLeadCommandHandler(ILeadRepository leadRepository, IServiceProvider serviceProvider, ILogger<UpdateLeadCommandHandler> logger) : base(serviceProvider, logger)
    {
        _leadRepository = leadRepository;
    }

    protected override async Task<ServiceResultDTO> HandleRequest(UpdateLeadCommand request, CancellationToken cancellationToken)
    {
        ServiceResultDTO serviceResult = new ServiceResultDTO();

        var lead = await _leadRepository.GetLeadById(new LeadId(request.LeadId));
        if (lead == null)
        {
            throw new EntityNotFoundException("Lead", request.LeadId.ToString());
        }

        lead.PhoneNumber = request.PhoneNumber ?? lead.PhoneNumber;
        lead.ProductName = request.ProductName ?? lead.ProductName;
        lead.GoogleLocationLink = request.GoogleLocationLink ?? lead.GoogleLocationLink;
        if (request.OrderDraftId.HasValue) lead.OrderDraftId = request.OrderDraftId.Value;
        if (request.OrderId.HasValue) lead.OrderId = request.OrderId.Value;
        lead.UpdatedBy = _currentUser.EmployeeId;
        lead.UpdatedOn = DateTime.UtcNow;

        await _leadRepository.UpdateLead(lead);

        return serviceResult;
    }
}
