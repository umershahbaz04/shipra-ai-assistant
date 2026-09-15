using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DocumentGenerator.ExcelReports;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DriverAccountFeatures.Query.ExcelExportDriverExpense;

public class ExcelExportDriverExpenseQueryHandler : RequestHandlerBase<ExcelExportDriverExpenseQuery, ServiceResultDTOWithTypeModel<ExcelResponseModel>>
{
  private readonly IDriverAccountRepository _driverAccount;

  public ExcelExportDriverExpenseQueryHandler(IDriverAccountRepository driverAccount, IServiceProvider serviceProvider, ILogger<ExcelExportDriverExpenseQueryHandler> logger) : base(serviceProvider, logger)
  {
    _driverAccount = driverAccount;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<ExcelResponseModel>> HandleRequest(ExcelExportDriverExpenseQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTOWithTypeModel<ExcelResponseModel> serviceResult = new ServiceResultDTOWithTypeModel<ExcelResponseModel>();
    try
    {
      var filter = request.FilterModel!;
      var expense = await _driverAccount.GetAllDriverExpense(_currentUser.ClientId!.Value.ToString(), filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!, request.DriverId!, request.ExpenseCategoryId);
      var excelDriverExpense = new ExportToExcelDriverExpense();
      var data = excelDriverExpense.ExportToExcel(expense!.list, "Expense Report");

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
