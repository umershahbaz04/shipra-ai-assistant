using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DocumentGenerator.ExcelReports;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Query.ExcelReportProductCountAvailability;
public class ExcelExportProductStockCountAvailableQuery : IRequest<ServiceResultDTOWithTypeModel<ExcelResponseModel>>
{
  public FilterModelDTO? FilterModel { get; set; }
  public string? ProductId { get; set; }
}
public class ExcelExportProductCountAvailableQueryHandler : RequestHandlerBase<ExcelExportProductStockCountAvailableQuery, ServiceResultDTOWithTypeModel<ExcelResponseModel>>
{
  private readonly IProductRepository _productRepository;
  public ExcelExportProductCountAvailableQueryHandler(IProductRepository productRepository, IServiceProvider serviceProvider, ILogger<ExcelExportProductCountAvailableQueryHandler> logger) : base(serviceProvider, logger)
  {
    _productRepository = productRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<ExcelResponseModel>> HandleRequest(ExcelExportProductStockCountAvailableQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTOWithTypeModel<ExcelResponseModel> serviceResult = new ServiceResultDTOWithTypeModel<ExcelResponseModel>();
    try
    {
      var filter = request.FilterModel!;
      var productStockCountAvailability = await _productRepository.GetProductStockCountAvailability(request.ProductId!, filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!,_currentUser.ClientIdStr!);
      var productCount = new ExportToExcelProductStockCountAvailable();
      var data = productCount.ExportToExcel(productStockCountAvailability, "ProductStock Count Report");

      serviceResult = new ServiceResultDTOWithTypeModel<ExcelResponseModel>(new ExcelResponseModel() { Bytes = data });

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
