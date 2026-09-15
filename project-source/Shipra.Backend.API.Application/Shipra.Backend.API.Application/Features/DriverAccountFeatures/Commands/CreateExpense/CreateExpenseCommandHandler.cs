using System.Net;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.DriverAccountFeatures.Commands.CreateExpense;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DriverAccountFeatures.Commands.CreateDriverExpense;

public class CreateExpenseCommandHandler : RequestHandlerBase<CreateExpenseCommand, ServiceResultDTO>
{
  private readonly IDriverAccountRepository _driverExpense;

  public CreateExpenseCommandHandler(IDriverAccountRepository driverExpense, IServiceProvider serviceProvider, ILogger<CreateExpenseCommandHandler> logger) : base(serviceProvider, logger)
  {
    _driverExpense = driverExpense;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateExpenseCommand request, CancellationToken cancellationToken)
  {
    var response = new ServiceResultDTO();

    try
    {
      var oDriverExpense = Expense.CreateExpense(_currentUser.ClientId!, new DriverId(new Guid(request.DriverId!)), new DriverReceivableId(new Guid(request!.DriverReceiveableId!)), new Core.DeliveryNoteAggregate.DeliveryNoteId(new Guid(request.DeliveryNoteId!)), request.Amount, request.ExpenseDate, request.ExpenseCategoryId, request.Details, _currentUser.EmployeeId!);
      var driverExpense = await _driverExpense.CreateExpense(oDriverExpense);
      var result = new BaseResponseDto()
      {
        Data = driverExpense,
        Message = NotificationConstants.SavedSuccess
      };
      response.CreateSuccessResponse(HttpStatusCode.OK);
      return response;
    }
    catch (Exception ex)
    {
      response.CreateErrorResponse(ex);
      throw;
    }
  }
}
