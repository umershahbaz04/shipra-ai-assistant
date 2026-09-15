using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DriverAccountFeatures.Commands.UpdateDriverExpense;

public class UpdateExpenseCommandHandler : RequestHandlerBase<UpdateExpenseCommand, ServiceResultDTOWithTypeModel<BaseResponseDto>>
{
  private readonly IDriverAccountRepository _driverExpense;

  public UpdateExpenseCommandHandler(IDriverAccountRepository driverExpense, IServiceProvider serviceProvider, ILogger<UpdateExpenseCommandHandler> logger) : base(serviceProvider, logger)
  {
    _driverExpense = driverExpense;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<BaseResponseDto>> HandleRequest(UpdateExpenseCommand request, CancellationToken cancellationToken)
  {
    var response = new ServiceResultDTOWithTypeModel<BaseResponseDto>();

    try
    {
      var expense = await _driverExpense.GetExpenseById(new ExpenseId(new Guid(request.ExpenseId!)));

      if (expense is null)
      {
        throw new EntityNotFoundException("DriverExpense", request.ExpenseId!);
      }

      expense.UpdateExpense(_currentUser.ClientId!, new DriverId(new Guid(request.DriverId!)), request.Amount, request.ExpenseDate, request.ExpenseCategoryId, request.Details, _currentUser.EmployeeId!);
      var isExpenseUpdate = await _driverExpense.UpdateExpense(expense);
      if (isExpenseUpdate is not null)
      {
        response.CreateSuccessResponse();
      }
      return response;
    }
    catch (Exception ex)
    {
      response.CreateErrorResponse(ex);
      throw;
    }
  }
}
