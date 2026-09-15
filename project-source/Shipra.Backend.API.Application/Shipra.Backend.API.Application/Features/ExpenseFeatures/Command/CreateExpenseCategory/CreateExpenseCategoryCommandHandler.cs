using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.ExpenseAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ExpenseFeatures.Command.CreateExpenseCategory;
public class CreateExpenseCategoryCommandHandler : RequestHandlerBase<CreateExpenseCategoryCommand, ServiceResultDTO>
{
  private readonly IExpenseRepository _expenseRepository;

  public CreateExpenseCategoryCommandHandler(IExpenseRepository expenseRepository, IServiceProvider serviceProvider, ILogger<CreateExpenseCategoryCommandHandler> logger) : base(serviceProvider, logger)
  {
    _expenseRepository = expenseRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateExpenseCategoryCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {

      var expenseCategory = await _expenseRepository.CreateExpenseCategory(ExpenseCategory.CreateExpenseCategory(_currentUser.ClientId!, request.ExpenseName!, _currentUser.EmployeeId!));
      if (expenseCategory is not null)
      {
        var dt = new BaseResponseDto()
        {
          Data = expenseCategory!.ExpenseCategoryId,
          Message = NotificationConstants.SavedSuccess
        };
        serviceResult = new ServiceResultDTO(dt);
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
