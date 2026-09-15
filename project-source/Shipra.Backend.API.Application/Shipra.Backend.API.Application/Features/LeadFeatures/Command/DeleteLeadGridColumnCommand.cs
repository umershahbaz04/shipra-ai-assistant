using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.LeadFeatures.Command;

public class DeleteLeadGridColumnCommand : IRequest<ServiceResultDTO>
{
    public int LeadGridColumnId { get; set; }
}

public class DeleteLeadGridColumnCommandHandler : RequestHandlerBase<DeleteLeadGridColumnCommand, ServiceResultDTO>
{
    private readonly ILeadRepository _leadRepository;

    public DeleteLeadGridColumnCommandHandler(ILeadRepository leadRepository, IServiceProvider serviceProvider, ILogger<DeleteLeadGridColumnCommandHandler> logger) : base(serviceProvider, logger)
    {
        _leadRepository = leadRepository;
    }

    protected override async Task<ServiceResultDTO> HandleRequest(DeleteLeadGridColumnCommand request, CancellationToken cancellationToken)
    {
        ServiceResultDTO serviceResult = new ServiceResultDTO();
        try
        {
            var column = await _leadRepository.GetLeadGridColumnById(request.LeadGridColumnId, _currentUser.ClientId!);
            if (column is null)
                throw new EntityNotFoundException("LeadGridColumn", request.LeadGridColumnId);

            if (column.IsDefaultStatusTab.GetValueOrDefault())
            {
                serviceResult.CreateError("DefaultTab", new[] { "You can't delete the default tab." });
                return serviceResult;
            }

            bool deleted = await _leadRepository.DeleteLeadGridColumn(column);
            serviceResult.IsSuccess = deleted;
            return serviceResult;
        }
        catch (Exception ex)
        {
            serviceResult.CreateErrorResponse(ex);
            throw;
        }
    }
}
