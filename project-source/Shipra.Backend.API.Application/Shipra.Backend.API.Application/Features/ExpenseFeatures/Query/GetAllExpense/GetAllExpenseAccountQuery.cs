using MediatR;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;

namespace Shipra.Backend.API.Application.Features.ExpenseFeatures.Query.GetAllExpense;
public class GetAllExpenseAccountQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
}
