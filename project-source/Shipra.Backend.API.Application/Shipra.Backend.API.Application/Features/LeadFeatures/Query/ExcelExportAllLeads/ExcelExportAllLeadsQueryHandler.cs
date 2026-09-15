using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DocumentGenerator.ExcelReports;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.LeadFeatures.Query.ExcelExportAllLeads;

public class ExcelExportAllLeadsQueryHandler : RequestHandlerBase<ExcelExportAllLeadsQuery, ServiceResultDTOWithTypeModel<ExcelResponseModel>>
{
  private readonly ILeadRepository _leadRepository;

  public ExcelExportAllLeadsQueryHandler(ILeadRepository leadRepository, IServiceProvider serviceProvider, ILogger<ExcelExportAllLeadsQueryHandler> logger) : base(serviceProvider, logger)
  {
    _leadRepository = leadRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<ExcelResponseModel>> HandleRequest(ExcelExportAllLeadsQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTOWithTypeModel<ExcelResponseModel> serviceResult = new ServiceResultDTOWithTypeModel<ExcelResponseModel>();
    try
    {
      var clientId = _currentUser.ClientId!.Value.ToString();
      dynamic leads = await _leadRepository.GetAllLeads(clientId, request.Start, request.Length, request.Search, request.SortCol, request.SortDir, request.LeadStatusIds, request.SalespersonIds, request.AssignmentFilter, request.CountryId);
      
      var excelLeads = new ExportToExcelLeads(); 

      var resultList = leads as IEnumerable<dynamic>;
      var data = excelLeads.ExportToExcel(resultList != null ? resultList.ToList() : new List<dynamic>(), "Leads Report");
      
      serviceResult = new ServiceResultDTOWithTypeModel<ExcelResponseModel>(new ExcelResponseModel { Bytes = data });
      serviceResult.CreateSuccessResponse();
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
