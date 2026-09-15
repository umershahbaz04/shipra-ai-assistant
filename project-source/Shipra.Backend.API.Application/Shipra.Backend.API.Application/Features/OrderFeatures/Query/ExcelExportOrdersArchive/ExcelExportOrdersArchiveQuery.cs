using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DocumentGenerator.ExcelReports;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetAllArchiveOrders;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Helper;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.ExcelExportOrdersArchive;
public class ExcelExportOrdersArchiveQuery : IRequest<ServiceResultDTO>
{
  public DateTime? Date { get; set; }
}
public class ExcelExportOrdersArchiveQueryHandler : RequestHandlerBase<ExcelExportOrdersArchiveQuery, ServiceResultDTO>
{
  private readonly IOrderRepository _orderRepository;

  public ExcelExportOrdersArchiveQueryHandler(IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<ExcelExportOrdersArchiveQueryHandler> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(ExcelExportOrdersArchiveQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var order = await _orderRepository.ExcelExportOrdersArchive(_currentUser.ClientIdStr!, request.Date);
      var excelShipments = new ExportToExcelCommon();
      string[] columnsNotToTake = { "OrderId", "RowNum", "TotalCount", "ItemValue", "Discount", "VAT", "StoreImage", "ClientName","ClientId", "PaymentLinkUrl", "IsMetaFieldExist" };
      var data = excelShipments.ExportToExcelWithDynamicList(order.list, "Settlement Report", columnsNotToTake);
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
