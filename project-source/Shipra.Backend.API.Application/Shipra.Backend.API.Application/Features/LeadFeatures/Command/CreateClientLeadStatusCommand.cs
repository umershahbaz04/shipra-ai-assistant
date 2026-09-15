using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.LeadAggregate;

namespace Shipra.Backend.API.Application.Features.LeadFeatures.Command;

public class CreateClientLeadStatusCommand : IRequest<ServiceResultDTO>
{
    public string? Description { get; set; }
    public int? DisplayOrder { get; set; }
}

public class CreateClientLeadStatusCommandHandler : RequestHandlerBase<CreateClientLeadStatusCommand, ServiceResultDTO>
{
    private readonly ILeadRepository _leadRepository;

    public CreateClientLeadStatusCommandHandler(ILeadRepository leadRepository, IServiceProvider serviceProvider, ILogger<CreateClientLeadStatusCommandHandler> logger) : base(serviceProvider, logger)
    {
        _leadRepository = leadRepository;
    }

    protected override async Task<ServiceResultDTO> HandleRequest(CreateClientLeadStatusCommand request, CancellationToken cancellationToken)
    {
        var serviceResult = new ServiceResultDTO();
        try
        {
            var entity = ClientLeadStatusLookup.Create(request.Description!, request.DisplayOrder, _currentUser.ClientId!, _currentUser.EmployeeId!);
            await _leadRepository.CreateClientLeadStatus(entity);
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
