using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DocumentGenerator.ExcelReports;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Application.DTOs.ProductUseCase.Request;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Query.ExcelExportProductInventorySummary;
public class ExcelExportProductInventorySummaryQuery : ProductInventoryFilterModel,IRequest<ServiceResultDTOWithTypeModel<ExcelResponseModel>>
{  
}
public class ExcelExportProductInventorySummaryQueryHandler : RequestHandlerBase<ExcelExportProductInventorySummaryQuery, ServiceResultDTOWithTypeModel<ExcelResponseModel>>
{
  private readonly IProductRepository _productRepository;
  public ExcelExportProductInventorySummaryQueryHandler(IProductRepository productRepository, IServiceProvider serviceProvider, ILogger<ExcelExportProductInventorySummaryQueryHandler> logger) : base(serviceProvider, logger)
  {
    _productRepository = productRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<ExcelResponseModel>> HandleRequest(ExcelExportProductInventorySummaryQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTOWithTypeModel<ExcelResponseModel> serviceResult = new ServiceResultDTOWithTypeModel<ExcelResponseModel>();
    try
    {
      var filter = request.FilterModel!;
      var productInventorySummary = await _productRepository.GetAllProductInventorySummaryAsync(request.StoreId, request.ProductStationId, request.IsActive, request.IsAvailable, request.AvailableQty, _currentUser.ClientIdStr!, filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!);
      var excelProductSummary = new ExportToExcelProductInventorySummary();
      var data = excelProductSummary.ExportToExcel(productInventorySummary, "ProductInventorySummary Report");
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
