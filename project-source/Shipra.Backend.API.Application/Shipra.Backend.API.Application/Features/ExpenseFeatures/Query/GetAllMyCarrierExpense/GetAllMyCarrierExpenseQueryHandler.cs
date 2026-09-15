using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ExpenseFeatures.Query.GetAllMyCarrierExpense;
public class GetAllMyCarrierExpenseQueryHandler : RequestHandlerBase<GetAllMyCarrierExpenseQuery, ServiceResultDTO>
{
  private readonly IExpenseRepository _expenseRepository;

  public GetAllMyCarrierExpenseQueryHandler(IExpenseRepository expenseRepository, IServiceProvider serviceProvider, ILogger<GetAllMyCarrierExpenseQueryHandler> logger) : base(serviceProvider, logger)
  {
    _expenseRepository = expenseRepository;
  }

  protected override Task<ServiceResultDTO> HandleRequest(GetAllMyCarrierExpenseQuery request, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}

