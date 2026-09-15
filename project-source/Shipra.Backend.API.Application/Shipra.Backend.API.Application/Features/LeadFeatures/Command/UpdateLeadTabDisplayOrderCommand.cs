using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.LeadFeatures.Command;

public class UpdateLeadTabDisplayOrderItem
{
    public int LeadGridColumnId { get; set; }
    public int DisplayOrder { get; set; }
}

public class UpdateLeadTabDisplayOrderCommand : IRequest<ServiceResultDTO>
{
    public List<UpdateLeadTabDisplayOrderItem>? List { get; set; }
}

public class UpdateLeadTabDisplayOrderCommandHandler : RequestHandlerBase<UpdateLeadTabDisplayOrderCommand, ServiceResultDTO>
{
    private readonly ILeadRepository _leadRepository;

    public UpdateLeadTabDisplayOrderCommandHandler(ILeadRepository leadRepository, IServiceProvider serviceProvider, ILogger<UpdateLeadTabDisplayOrderCommandHandler> logger) : base(serviceProvider, logger)
    {
        _leadRepository = leadRepository;
    }

    protected override async Task<ServiceResultDTO> HandleRequest(UpdateLeadTabDisplayOrderCommand request, CancellationToken cancellationToken)
    {
        var serviceResult = new ServiceResultDTO();
        try
        {
            if (request.List == null || request.List.Count == 0)
                return serviceResult;

            await _leadRepository.UpdateLeadTabDisplayOrder(request.List!.Select(x => (x.LeadGridColumnId, x.DisplayOrder)).ToList());
            return serviceResult;
        }
        catch (Exception ex)
        {
            serviceResult.CreateErrorResponse(ex);
            throw;
        }
    }
}
