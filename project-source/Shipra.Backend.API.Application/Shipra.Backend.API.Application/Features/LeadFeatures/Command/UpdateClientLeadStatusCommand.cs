using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.LeadFeatures.Command;

public class UpdateClientLeadStatusCommand : IRequest<ServiceResultDTO>
{
    public int ClientLeadStatusId { get; set; }
    public string? Description { get; set; }
    public int? DisplayOrder { get; set; }
}

public class UpdateClientLeadStatusCommandHandler : RequestHandlerBase<UpdateClientLeadStatusCommand, ServiceResultDTO>
{
    private readonly ILeadRepository _leadRepository;

    public UpdateClientLeadStatusCommandHandler(ILeadRepository leadRepository, IServiceProvider serviceProvider, ILogger<UpdateClientLeadStatusCommandHandler> logger) : base(serviceProvider, logger)
    {
        _leadRepository = leadRepository;
    }

    protected override async Task<ServiceResultDTO> HandleRequest(UpdateClientLeadStatusCommand request, CancellationToken cancellationToken)
    {
        ServiceResultDTO serviceResult = new ServiceResultDTO();
        try
        {
            var entity = await _leadRepository.GetClientLeadStatusById(request.ClientLeadStatusId, _currentUser.ClientId!);
            if (entity == null)
                throw new EntityNotFoundException("ClientLeadStatus", request.ClientLeadStatusId);

            entity.Update(request.Description!, request.DisplayOrder, _currentUser.EmployeeId!);
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
