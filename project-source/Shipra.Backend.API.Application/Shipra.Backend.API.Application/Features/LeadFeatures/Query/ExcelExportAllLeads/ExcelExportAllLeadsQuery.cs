using MediatR;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Response;

namespace Shipra.Backend.API.Application.Features.LeadFeatures.Query.ExcelExportAllLeads;
public class ExcelExportAllLeadsQuery : IRequest<ServiceResultDTOWithTypeModel<ExcelResponseModel>>
{
    public int Start { get; set; }
    public int Length { get; set; }
    public string? Search { get; set; }
    public int SortCol { get; set; }
    public string? SortDir { get; set; }
    public string? LeadStatusIds { get; set; }
    public string? SalespersonIds { get; set; }
    public string? AssignmentFilter { get; set; }
    public string? CountryId { get; set; }
    
}
