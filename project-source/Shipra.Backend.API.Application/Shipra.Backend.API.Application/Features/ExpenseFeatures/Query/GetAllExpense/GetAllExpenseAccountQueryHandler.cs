using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ExpenseFeatures.Query.GetAllExpense;
public class GetAllExpenseAccountQueryHandler : RequestHandlerBase<GetAllExpenseAccountQuery, ServiceResultDTO>
{
  private readonly IExpenseRepository _expenseRepository;

  public GetAllExpenseAccountQueryHandler(IExpenseRepository expenseRepository, IServiceProvider serviceProvider, ILogger<GetAllExpenseAccountQueryHandler> logger) : base(serviceProvider, logger)
  {
    _expenseRepository = expenseRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllExpenseAccountQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request?.FilterModel!;
      var oExpenseList = await _expenseRepository.GetAllExpenseAccount(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search, filter.SortCol, filter.SortDir, _currentUser.ClientId!.Value.ToString());
      if (oExpenseList is not null)
      {
        serviceResult = new ServiceResultDTO(oExpenseList);
        serviceResult.CreateSuccessResponse(System.Net.HttpStatusCode.OK);
      }
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
