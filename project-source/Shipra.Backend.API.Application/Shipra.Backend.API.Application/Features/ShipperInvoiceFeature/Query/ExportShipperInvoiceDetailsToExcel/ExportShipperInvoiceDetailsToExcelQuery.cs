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
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Query.ExportShipperInvoiceDetailsToExcel;
public class ExportShipperInvoiceDetailsToExcelQuery : IRequest<ServiceResultDTO>
{
  public int? ShipperInvoiceId { get; set; }
}
public class ExportShipperInvoiceDetailsToExcelQueryHandler : RequestHandlerBase<ExportShipperInvoiceDetailsToExcelQuery, ServiceResultDTO>
{
  private readonly IShipperInvoiceRepository _shipperInvoiceRepository;

  public ExportShipperInvoiceDetailsToExcelQueryHandler(IShipperInvoiceRepository shipperInvoiceRepository,IServiceProvider serviceProvider, ILogger<ExportShipperInvoiceDetailsToExcelQueryHandler> logger) : base(serviceProvider, logger)
  {
    _shipperInvoiceRepository = shipperInvoiceRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(ExportShipperInvoiceDetailsToExcelQuery request, CancellationToken cancellationToken)
  {
    var result = new ServiceResultDTO();

    try
    {
      var dataList = await _shipperInvoiceRepository.GetAllShipperInvoiceDetail(_currentUser.ClientIdStr,request.ShipperInvoiceId);
      
      
      var excelShipments = new ExportToExcelCommon();
      string[] columnsNotToTake = { "ShipperInvoiceDetailId", "ShipperOrderId", "OrderId" };
      var data = excelShipments.ExportToExcelWithDynamicList(dataList, "Transaction Report", columnsNotToTake);
      result = new ServiceResultDTO(new ExcelResponseModel { Bytes = data }); 
      return result;
    }
    catch (Exception ex)
    {
      result.CreateErrorResponse(ex);
      return result;
    }
  }
}
public class ExportShipperInvoiceDetailsToExcelQueryValidator : AbstractValidator<ExportShipperInvoiceDetailsToExcelQuery>
{
  public ExportShipperInvoiceDetailsToExcelQueryValidator()
  {
    RuleFor(x => x.ShipperInvoiceId).NotNull().NotEmpty().GreaterThan(0);
  } 
}
