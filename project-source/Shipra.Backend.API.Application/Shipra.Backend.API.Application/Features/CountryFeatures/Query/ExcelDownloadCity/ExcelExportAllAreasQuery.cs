using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DocumentGenerator.ExcelReports;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CountryFeatures.Query.ExcelDownloadCity;
public class ExcelExportAllAreasQuery : IRequest<ServiceResultDTO>
{
}
public class ExcelExportAllAreasQueryHandler : RequestHandlerBase<ExcelExportAllAreasQuery, ServiceResultDTO>
{
  private readonly ICountryRepository _countryRepository;

  public ExcelExportAllAreasQueryHandler(ICountryRepository countryRepository,IServiceProvider serviceProvider, ILogger<ExcelExportAllAreasQueryHandler> logger) : base(serviceProvider, logger)
  {
    _countryRepository = countryRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(ExcelExportAllAreasQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var queryData = await _countryRepository.GetAllAreas();

      var excel = new ExportToExcelCommon();

      var data = excel.ExportToExcel(queryData.Select(x => new
      {
        x.Code,
        x.Name,
      }).ToList(), "area list");
      serviceResult = new ServiceResultDTO(new ExcelResponseModel { Bytes = data });
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
