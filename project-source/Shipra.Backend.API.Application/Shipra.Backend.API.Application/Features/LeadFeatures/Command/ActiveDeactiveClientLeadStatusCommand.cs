using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.LeadFeatures.Command;

public class ActiveDeactiveClientLeadStatusCommand : IRequest<ServiceResultDTO>
{
    public int ClientLeadStatusId { get; set; }
    public bool? IsActive { get; set; }
}

public class ActiveDeactiveClientLeadStatusCommandHandler : RequestHandlerBase<ActiveDeactiveClientLeadStatusCommand, ServiceResultDTO>
{
    private readonly ILeadRepository _leadRepository;

    public ActiveDeactiveClientLeadStatusCommandHandler(ILeadRepository leadRepository, IServiceProvider serviceProvider, ILogger<ActiveDeactiveClientLeadStatusCommandHandler> logger) : base(serviceProvider, logger)
    {
        _leadRepository = leadRepository;
    }

    protected override async Task<ServiceResultDTO> HandleRequest(ActiveDeactiveClientLeadStatusCommand request, CancellationToken cancellationToken)
    {
        var serviceResult = new ServiceResultDTO();
        try
        {
            var entity = await _leadRepository.GetClientLeadStatusById(request.ClientLeadStatusId, _currentUser.ClientId!);
            if (entity == null)
                throw new EntityNotFoundException("ClientLeadStatus", request.ClientLeadStatusId);

            entity.ActivateDeactive(_currentUser.EmployeeId!, request.IsActive ?? false);
            await _leadRepository.UpdateClientLeadStatus(entity);
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
