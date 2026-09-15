using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DocumentGenerator.ExcelReports;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CountryFeatures.Query.ExcelDownloadCountry;
public class ExcelExportAllCountryQuery : IRequest<ServiceResultDTO>
{
}
public class ExcelExportAllCountryQueryHandler : RequestHandlerBase<ExcelExportAllCountryQuery, ServiceResultDTO>
{
  private readonly ICountryRepository _countryRepository;

  public ExcelExportAllCountryQueryHandler(ICountryRepository countryRepository, IServiceProvider serviceProvider, ILogger<ExcelExportAllCountryQueryHandler> logger) : base(serviceProvider, logger)
  {
    _countryRepository = countryRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(ExcelExportAllCountryQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var queryData = await _countryRepository.GetAllCountries();

      var excel = new ExportToExcelCommon();

      var data = excel.ExportToExcel(queryData.Select(x => new
      {
        x.Code,
        x.Name,
      }).ToList(), "country list");
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
