using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DocumentGenerator.ExcelReports;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Application.DTOs.ProductUseCase.Request;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Query.ExcekExportLowQuantityProductStock;
public class ExcekExportLowQuantityProductStockQuery : LowQuantityProductStockFilterModel,IRequest<ServiceResultDTO>
{

}
public class ExcekExportLowQuantityProductStockQueryHandler : RequestHandlerBase<ExcekExportLowQuantityProductStockQuery, ServiceResultDTO>
{
  private readonly IProductRepository _productRepository;

  public ExcekExportLowQuantityProductStockQueryHandler(IProductRepository productRepository, IServiceProvider serviceProvider, ILogger<ExcekExportLowQuantityProductStockQueryHandler> logger) : base(serviceProvider, logger)
  {
    _productRepository = productRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(ExcekExportLowQuantityProductStockQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request.FilterModel!;

      dynamic oResult = await _productRepository.GetAllLowQuantityProductStock(request.StoreId, request.ProductStationId, request.IsActive,request.AvailableQty, _currentUser.ClientIdStr!, filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!);

      var inventorySalesExcel = new ExportToExcelLowQuantityProductStock();
      var data = inventorySalesExcel.ExportToExcel(oResult?.list, "Low Inventory");
      serviceResult = new ServiceResultDTO(new ExcelResponseModel() { Bytes = data });

      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
