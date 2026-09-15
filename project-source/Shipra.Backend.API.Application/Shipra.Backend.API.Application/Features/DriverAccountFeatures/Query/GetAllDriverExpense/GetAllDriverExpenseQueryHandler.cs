using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DriverAccountFeatures.Query.GetAllDriverExpense;

public class GetAllDriverExpenseQueryHandler : RequestHandlerBase<GetAllDriverExpenseQuery, ServiceResultDTO>
{
  private readonly IDriverAccountRepository _driverExpense;

  public GetAllDriverExpenseQueryHandler(IDriverAccountRepository driverExpense, IServiceProvider serviceProvider, ILogger<GetAllDriverExpenseQueryHandler> logger) : base(serviceProvider, logger)
  {
    _driverExpense = driverExpense;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllDriverExpenseQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request.FilterModel!;

      var oDriverExpense = await _driverExpense.GetAllDriverExpense(_currentUser.ClientId!.Value.ToString(), filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!, request.DriverId!, request.ExpenseCategoryId);
      if (oDriverExpense is not null)
      {
        serviceResult = new ServiceResultDTO(oDriverExpense);

        serviceResult.CreateSuccessResponse();
      }
      else
      {
        serviceResult.CreateErrorResponse(System.Net.HttpStatusCode.NotFound);
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
