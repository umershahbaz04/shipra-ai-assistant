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
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.AccountFeature.Query.ExcelExportCarrierSettlementById;
public class ExcelExportCarrierSettlementByIdQuery : IRequest<ServiceResultDTO>
{
  public string? CarrierPaymentSettlementId { get; set; }
}
public class ExcelExportCarrierSettlementByIdQueryHandler : RequestHandlerBase<ExcelExportCarrierSettlementByIdQuery, ServiceResultDTO>
{
  private readonly IAccountRepository _accountRepository;

  public ExcelExportCarrierSettlementByIdQueryHandler(IAccountRepository accountRepository, IServiceProvider serviceProvider, ILogger<ExcelExportCarrierSettlementByIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _accountRepository = accountRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(ExcelExportCarrierSettlementByIdQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var clientId = _currentUser.ClientId!.Value.ToString();

      dynamic reportData = await _accountRepository.GetShipmentsBySettlementId(request.CarrierPaymentSettlementId!, clientId);


      var excelShipments = new ExportToExcelCommon();
      string[] columnsNotToTake = { "OrderId","RowNum", "TotalCount", "ItemValue", "Discount", "VAT", "StoreImage", "ClientName" };
      var data = excelShipments.ExportToExcelWithDynamicList(reportData.list, "Settlement Report", columnsNotToTake);
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
