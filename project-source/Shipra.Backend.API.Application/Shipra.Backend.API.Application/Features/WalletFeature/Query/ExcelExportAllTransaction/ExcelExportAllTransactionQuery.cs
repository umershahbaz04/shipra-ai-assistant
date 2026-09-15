using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DocumentGenerator.ExcelReports;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.WalletFeature.Query.ExcelExportAllTransaction;
public class ExcelExportAllTransactionQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
  public int? TransactionTypeId { get; set; } = 0;
}
public class ExcelExportAllTransactionQueryHandler : RequestHandlerBase<ExcelExportAllTransactionQuery, ServiceResultDTO>
{
  private readonly IWalletRepository _walletRepository;

  public ExcelExportAllTransactionQueryHandler(IWalletRepository walletRepository,IServiceProvider serviceProvider, ILogger<ExcelExportAllTransactionQueryHandler> logger) : base(serviceProvider, logger)
  {
    _walletRepository = walletRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(ExcelExportAllTransactionQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request.FilterModel!;
      var clientId = _currentUser.ClientId!.Value.ToString();
      dynamic allTrans = await _walletRepository.GetAllTransaction(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!, clientId, request.TransactionTypeId);
       
      var excelShipments = new ExportToExcelCommon();
      string[] columnsNotToTake = { "RowNum", "TotalCount", "TransactionId", "WalletId", "TransactionTypeId" };
      var data = excelShipments.ExportToExcelWithDynamicList(allTrans.list, "Transaction Report", columnsNotToTake);
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
