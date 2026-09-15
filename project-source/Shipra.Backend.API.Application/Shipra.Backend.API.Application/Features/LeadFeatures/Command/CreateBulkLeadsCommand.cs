using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.LeadAggregate;
using System.Net;

namespace Shipra.Backend.API.Application.Features.LeadFeatures.Command;

public class CreateBulkLeadsCommand : IRequest<ServiceResultDTO>
{
    public List<LeadUploadDTO> Leads { get; set; } = new List<LeadUploadDTO>();
}

public class CreateBulkLeadsCommandHandler : RequestHandlerBase<CreateBulkLeadsCommand, ServiceResultDTO>
{
    private readonly ILeadRepository _leadRepository;
    private readonly IOrderRepository _orderRepository;

    public CreateBulkLeadsCommandHandler(ILeadRepository leadRepository, IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<CreateBulkLeadsCommandHandler> logger) : base(serviceProvider, logger)
    {
        _leadRepository = leadRepository;
        _orderRepository = orderRepository;
    }

    protected override async Task<ServiceResultDTO> HandleRequest(CreateBulkLeadsCommand request, CancellationToken cancellationToken)
    {
        ServiceResultDTO serviceResult = new ServiceResultDTO();
        try
        {
            if (request.Leads == null || !request.Leads.Any())
            {
                serviceResult.IsSuccess = false;
                serviceResult.StatusCode = (int)HttpStatusCode.BadRequest;
                serviceResult.Errors?.Add("InvalidParameter", new[] { "No leads to create." });
                return serviceResult;
            }

            var clientId = new ClientId(_currentUser.ClientId!.Value);
            int defaultClientLeadStatusId = await _leadRepository.GetDefaultClientLeadStatusId(clientId);

            var leadMobiles = request.Leads
                .Where(x => !string.IsNullOrEmpty(x.PhoneNumber) && x.PhoneNumber.Length >= 8)
                .Select(x => x.PhoneNumber!)
                .Distinct()
                .ToList();

            var bulkDuplicates = await _orderRepository.CheckMobileNosDuplicateBulk(leadMobiles, _currentUser?.ClientIdStr ?? "");

            var failedLeads = new System.Collections.Generic.List<string>();
            var failedLeadsWithStatus = new System.Collections.Generic.List<(string Phone, string Status)>();
            foreach (var item in request.Leads)
            {
                var existingStatus = await _leadRepository.GetNonCompletedLeadStatusByPhone(clientId, item.PhoneNumber!);
                if (existingStatus != null)
                {
                    failedLeads.Add($"{item.PhoneNumber!} (Status: {existingStatus})");
                    failedLeadsWithStatus.Add((item.PhoneNumber!, existingStatus));
                    continue;
                }

                // Check duplicate mobile numbers validation checks
                if (!string.IsNullOrEmpty(item.PhoneNumber) && item.PhoneNumber.Length >= 8)
                {
                    var suffix = item.PhoneNumber.Substring(item.PhoneNumber.Length - 8);
                    var dupResult = bulkDuplicates.FirstOrDefault(d => d.IsDuplicate && d.OrderNo != null && 
                        d.MobileNo != null && d.MobileNo.EndsWith(suffix));

                    if (dupResult != null && dupResult.IsDuplicate)
                    {
                        failedLeads.Add($"{item.PhoneNumber!} (Reason: Duplicate Order Mobile {dupResult.OrderNo})");
                        failedLeadsWithStatus.Add((item.PhoneNumber!, $"Duplicate Order Mobile {dupResult.OrderNo}"));
                        continue;
                    }
                }

                var salespersonId = item.SalespersonId.HasValue ? new EmployeeId(item.SalespersonId.Value) : null;
                var currentEmployeeId = _currentUser?.EmployeeId != null ? new EmployeeId(_currentUser.EmployeeId.Value) : null;
                var newLead = Lead.CreateLead(clientId, item.PhoneNumber!, item.ProductName!, item.GoogleLocationLink ?? "", salespersonId, currentEmployeeId, defaultClientLeadStatusId, item.CountryId);
                await _leadRepository.CreateLead(newLead);
            }

            if (failedLeads.Any())
            {
                if (failedLeads.Count == request.Leads.Count)
                {
                    serviceResult.IsSuccess = false;
                    serviceResult.StatusCode = (int)HttpStatusCode.BadRequest;
                    if (request.Leads.Count == 1)
                    {
                        var singleFailed = failedLeadsWithStatus.First();
                        serviceResult.Errors?.Add("DuplicateOpenLead", new[] { $"Lead already exists with status: {singleFailed.Status}" });
                    }
                    else
                    {
                        serviceResult.Errors?.Add("DuplicateOpenLead", new[] { $"Lead(s) already exist with a non-Completed status: {string.Join(", ", failedLeads)}" });
                    }
                    return serviceResult;
                }
                else
                {
                    serviceResult = new ServiceResultDTO($"Leads created successfully. Note: Lead(s) already exist with a non-Completed status: {string.Join(", ", failedLeads)} and were skipped.");
                }
            }
            else
            {
                serviceResult = new ServiceResultDTO("Leads created successfully.");
            }
        }
        catch (Exception ex)
        {
            serviceResult.IsSuccess = false;
            serviceResult.Errors?.Add("Exception", new[] { ex.Message });
            serviceResult.StatusCode = (int)HttpStatusCode.InternalServerError;
        }

        return serviceResult;
    }
}
