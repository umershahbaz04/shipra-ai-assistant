using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DocumentGenerator.ExcelReports;
using Shipra.Backend.API.Application.DocumentGenerator.ExcelReports.HelperConvertModel;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Query.DownloadInventoryHistorySummaryExcel;
public class DownloadInventoryHistorySummaryExcelQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
  public long ProductStockId { get; set; }
  public int ReasonId { get; set; }
}
public class DownloadInventoryHistorySummaryExcelQueryHandler : RequestHandlerBase<DownloadInventoryHistorySummaryExcelQuery, ServiceResultDTO>
{
  private readonly IProductRepository _productRepository;

  public DownloadInventoryHistorySummaryExcelQueryHandler(IProductRepository productRepository, IServiceProvider serviceProvider, ILogger<DownloadInventoryHistorySummaryExcelQueryHandler> logger) : base(serviceProvider, logger)
  {
    _productRepository = productRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(DownloadInventoryHistorySummaryExcelQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request.FilterModel!;
      var oStockHistory = await _productRepository.GetProductStockHistoryByStockIdAsync(request.ProductStockId, request.ReasonId, filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!, _currentUser.ClientIdStr!);

      var inventoryBalance = await _productRepository.GetInventoryBalanceByIdAsync(request.ProductStockId);

      var castedType = (IEnumerable<dynamic>)oStockHistory.list;
      var typedList = castedType.Select(item =>
      new ProductStockAdjustmentHistoryByStockId
      {
        Comment = item.Comment,
        NewQuantity = item.NewQuantity,
        PreviousQuantity = item.PreviousQuantity,
        SKU = item.SKU,
        ProductStockHistorytId = item.ProductStockHistorytId,
        CreatedOn = item.CreatedOn,
        StoreName = item.StoreName
      }).ToList();

      var convertedData = InventoryHistorySaleReportModel.ConvertToViewModel(typedList);

      StockHistoryModel stockHistory = new StockHistoryModel(); 
      stockHistory.newExl = convertedData;
      if (inventoryBalance != null)
      {
        stockHistory.AvailableQty = inventoryBalance.QuantityAvailable;
        stockHistory.LastUpdated = inventoryBalance.UpdatedOn != null ? inventoryBalance.UpdatedOn : inventoryBalance.CreatedOn;
      }
      var report = new ExportInventoryHistorySummaryExcel();
      var data = report.ExportToExcelSummary(stockHistory); 
      serviceResult = new ServiceResultDTO(new ExcelResponseModel { Bytes = data });
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}


public class DownloadInventoryHistorySummaryExcelQueryValidator : AbstractValidator<DownloadInventoryHistorySummaryExcelQuery>
{
  public DownloadInventoryHistorySummaryExcelQueryValidator()
  {
    RuleFor(v => v.FilterModel!.Length).GreaterThan(0);
    RuleFor(v => v.ProductStockId).NotNull().NotEmpty().GreaterThan(0);
  }
}
