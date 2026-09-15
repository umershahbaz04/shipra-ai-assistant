using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.LeadAggregate;

namespace Shipra.Backend.API.Application.Features.LeadFeatures.Command;

public class DeleteBulkLeadsCommand : IRequest<ServiceResultDTO>
{
    public List<string> LeadIds { get; set; } = new List<string>();
}

public class DeleteBulkLeadsCommandHandler : RequestHandlerBase<DeleteBulkLeadsCommand, ServiceResultDTO>
{
    private readonly ILeadRepository _leadRepository;
    private readonly IOrderRepository _orderRepository;

    public DeleteBulkLeadsCommandHandler(
        ILeadRepository leadRepository, 
        IOrderRepository orderRepository, 
        IServiceProvider serviceProvider, 
        ILogger<DeleteBulkLeadsCommandHandler> logger) : base(serviceProvider, logger)
    {
        _leadRepository = leadRepository;
        _orderRepository = orderRepository;
    }

    protected override async Task<ServiceResultDTO> HandleRequest(DeleteBulkLeadsCommand request, CancellationToken cancellationToken)
    {
        var leadsToDelete = new List<Lead>();

        if (request.LeadIds == null || !request.LeadIds.Any())
        {
            var result = new ServiceResultDTO();
            result.CreateError("Error", new[] { "No leads provided for deletion." });
            return result;
        }

        foreach (var idString in request.LeadIds)
        {
            if (Guid.TryParse(idString, out Guid leadGuidId))
            {
                var lead = await _leadRepository.GetLeadById(new LeadId(leadGuidId));
                if (lead != null)
                {
                    // "we will never delete actual order against lead but we will delete draft order along with lead"
                    // So we do not delete actual order.
                    // But we DO delete draft order if it exists.
                    if (lead.OrderDraftId.HasValue && lead.ClientId != null)
                    {
                        var draft = await _orderRepository.GetDraftOrderById(lead.OrderDraftId.Value, lead.ClientId);
                        if (draft != null)
                        {
                            await _orderRepository.DeleteDraftOrder(draft);
                        }
                    }

                    leadsToDelete.Add(lead);
                }
            }
        }

        if (leadsToDelete.Any())
        {
            await _leadRepository.DeleteLeads(leadsToDelete);
            return new ServiceResultDTO(new { Message = $"{leadsToDelete.Count} lead(s) deleted successfully." }, true);
        }
        else
        {
            var result = new ServiceResultDTO();
            result.CreateError("Error", new[] { "No matching leads found to delete." });
            return result;
        }
    }
}
