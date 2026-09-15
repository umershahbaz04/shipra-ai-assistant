using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.DriverAccountFeatures.Commands.UpdateDriverExpense;
public class UpdateExpenseCommand : IRequest<ServiceResultDTOWithTypeModel<BaseResponseDto>>
{
  public string? ExpenseId { get; set; }
  public string? DriverId { get; set; }
  public decimal Amount { get; set; }
  public DateTime ExpenseDate { get; set; }
  public int ExpenseCategoryId { get; set; }
  public string? Details { get; set; }
}
