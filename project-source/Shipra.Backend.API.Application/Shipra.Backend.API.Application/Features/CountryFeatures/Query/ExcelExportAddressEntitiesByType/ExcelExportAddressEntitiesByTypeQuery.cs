using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DocumentGenerator.ExcelReports;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CountryFeatures.Query.ExcelExportAddressEntitiesByType;
public class ExcelExportAddressEntitiesByTypeQuery : IRequest<ServiceResultDTO>
{
  public int CountryId { get; set; }
  public string? NextEntityType { get; set; }
}
public class ExcelExportAddressEntitiesByTypeQueryHandler : RequestHandlerBase<ExcelExportAddressEntitiesByTypeQuery, ServiceResultDTO>
{
  private readonly ICountryRepository _countryRepository;

  public ExcelExportAddressEntitiesByTypeQueryHandler(ICountryRepository countryRepository, IServiceProvider serviceProvider, ILogger<ExcelExportAddressEntitiesByTypeQueryHandler> logger) : base(serviceProvider, logger)
  {
    _countryRepository = countryRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(ExcelExportAddressEntitiesByTypeQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      dynamic list = await _countryRepository.GetAddressEntitiesByTypeForExcel(request.CountryId, request.NextEntityType); 

      var excel = new ExportToExcelCommon();

      var data = excel.ExportToExcel(list, request.NextEntityType);
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
public class ExcelExportAddressEntitiesByTypeQueryValidator : AbstractValidator<ExcelExportAddressEntitiesByTypeQuery>
{
  public ExcelExportAddressEntitiesByTypeQueryValidator()
  {
    RuleFor(x => x.CountryId).NotEmpty().NotNull().GreaterThan(0);
    RuleFor(x => x.NextEntityType).NotEmpty().NotNull();
  }
}

