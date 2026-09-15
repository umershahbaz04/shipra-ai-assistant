using System.Net;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DriverAccountFeatures.Commands.DeleteExpense;
public class DeleteExpenseCommandHandler : RequestHandlerBase<DeleteExpenseCommand, ServiceResultDTOWithTypeModel<BaseResponseDto>>
{
  private readonly IDriverAccountRepository _driverExpense;

  public DeleteExpenseCommandHandler(IDriverAccountRepository driverExpense, IServiceProvider serviceProvider, ILogger<DeleteExpenseCommandHandler> logger) : base(serviceProvider, logger)
  {
    _driverExpense = driverExpense;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<BaseResponseDto>> HandleRequest(DeleteExpenseCommand request, CancellationToken cancellationToken)
  {
    var response = new ServiceResultDTOWithTypeModel<BaseResponseDto>();

    try
    {
      var expense = await _driverExpense.GetExpenseById(new ExpenseId(new Guid(request.ExpenseId!)));
      if (expense is null)
      {
        throw new EntityNotFoundException("DriverExpense", request.ExpenseId!);
      }
      await _driverExpense.DeleteExpense(expense);
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

