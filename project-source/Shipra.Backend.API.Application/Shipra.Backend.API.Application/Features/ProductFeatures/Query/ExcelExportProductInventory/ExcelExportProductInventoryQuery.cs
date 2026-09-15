using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DocumentGenerator.ExcelReports;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Application.DTOs.ProductUseCase.Request;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Query.ExcelExportProductInventory;
public class ExcelExportProductInventoryQuery : ProductInventoryFilterModel,IRequest<ServiceResultDTOWithTypeModel<ExcelResponseModel>>
{ 
}
public class ExcelExportProductInventoryQueryHandler : RequestHandlerBase<ExcelExportProductInventoryQuery, ServiceResultDTOWithTypeModel<ExcelResponseModel>>
{
  private readonly IProductRepository _productRepository;
  public ExcelExportProductInventoryQueryHandler(IProductRepository _productRepositor, IServiceProvider serviceProvider, ILogger<ExcelExportProductInventoryQueryHandler> logger) : base(serviceProvider, logger)
  {
    _productRepository = _productRepositor;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<ExcelResponseModel>> HandleRequest(ExcelExportProductInventoryQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTOWithTypeModel<ExcelResponseModel> serviceResult = new ServiceResultDTOWithTypeModel<ExcelResponseModel>();
    try
    {
      var filter = request.FilterModel!;
      var productInventory = await _productRepository.GetAllProductInventoryAsync(request.StoreId, request.ProductStationId, request.IsActive,request.IsAvailable,request.AvailableQty, _currentUser.ClientIdStr!, filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!);
      var Inventory = new ExportToExcelProductInventory();
      var data = Inventory.ExportToExcel(productInventory.list, "Product Inventory Report - " + Guid.NewGuid());

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
