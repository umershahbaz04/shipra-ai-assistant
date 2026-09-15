using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.DriverExpenseUseCase;
using Shipra.Backend.API.Application.Features.DriverAccountFeatures.Query.GetDriverExpenseById;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DriverAccountFeatures.Query.GetExpenseById;
public class GetExpenseByIdQueryHandler : RequestHandlerBase<GetExpenseByIdQuery, ServiceResultDTO>
{
  private readonly IDriverAccountRepository _driverExpense;

  public GetExpenseByIdQueryHandler(IDriverAccountRepository driverExpense, IServiceProvider serviceProvider, ILogger<GetExpenseByIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _driverExpense = driverExpense;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetExpenseByIdQuery request, CancellationToken cancellationToken)
  {
    var response = new ServiceResultDTO();

    try
    {
      var expense = await _driverExpense.GetExpenseById(new ExpenseId(new Guid(request!.ExpenseId!)));
      var mapper = _mapper.Map<DriverExpenseResponseModel>(expense);

      if (expense is null)
      {
        throw new EntityNotFoundException("DriverExpense", request.ExpenseId!);
      }
      response = new ServiceResultDTO(mapper);
      response.CreateSuccessResponse();
      return response;
    }
    catch (Exception ex)
    {
      response.CreateErrorResponse(ex);
      throw;
    }
  }
}
