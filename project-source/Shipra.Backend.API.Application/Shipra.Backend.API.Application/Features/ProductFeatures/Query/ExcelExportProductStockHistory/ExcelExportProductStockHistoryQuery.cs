using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DocumentGenerator.ExcelReports;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Query.ExcelExportDriverExpense;
public class ExcelExportProductStockHistoryQuery : IRequest<ServiceResultDTOWithTypeModel<ExcelResponseModel>>
{
  public FilterModelDTO? FilterModel { get; set; }
  public long ProductStockId { get; set; }
  public int ReasonId { get; set; }
}
public class ExcelExportProductStockHistoryQueryHandler : RequestHandlerBase<ExcelExportProductStockHistoryQuery,
  ServiceResultDTOWithTypeModel<ExcelResponseModel>>
{
  private readonly IProductRepository _productRepository;
  public ExcelExportProductStockHistoryQueryHandler(IProductRepository productRepository, IServiceProvider serviceProvider, ILogger<ExcelExportProductStockHistoryQueryHandler> logger) : base(serviceProvider, logger)
  {
    _productRepository = productRepository;
  }
  protected override async Task<ServiceResultDTOWithTypeModel<ExcelResponseModel>> HandleRequest(ExcelExportProductStockHistoryQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTOWithTypeModel<ExcelResponseModel> serviceResult = new ServiceResultDTOWithTypeModel<ExcelResponseModel>();
    try
    {

      var filter = request.FilterModel!;
      var stockHistory = await _productRepository.GetProductStockHistoryByStockIdAsync(request.ProductStockId, request.ReasonId, filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!,_currentUser.ClientIdStr!);
      var productStockHistory = new ExportToExcelProductStockHistory();
      var data = productStockHistory.ExportToExcel(stockHistory.list, "Product Stock History Report");

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
