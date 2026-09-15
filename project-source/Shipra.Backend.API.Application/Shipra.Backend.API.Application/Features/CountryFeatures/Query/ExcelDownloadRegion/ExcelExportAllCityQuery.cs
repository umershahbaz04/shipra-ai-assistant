using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DocumentGenerator.ExcelReports;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CountryFeatures.Query.ExcelDownloadRegion;
public class ExcelExportAllCityQuery : IRequest<ServiceResultDTO>
{
}
public class ExcelExportAllCityQueryHandler : RequestHandlerBase<ExcelExportAllCityQuery, ServiceResultDTO>
{
  private readonly ICountryRepository _countryRepository;

  public ExcelExportAllCityQueryHandler(ICountryRepository countryRepository, IServiceProvider serviceProvider, ILogger<ExcelExportAllCityQueryHandler> logger) : base(serviceProvider, logger)
  {
    _countryRepository = countryRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(ExcelExportAllCityQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    { 
      var queryData = await _countryRepository.GetAllCities();

      var excel = new ExportToExcelCommon(); 

      var data = excel.ExportToExcel(queryData.Select(x => new
      {
        x.Code,
        x.Name,
      }).ToList(), "City list");
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
