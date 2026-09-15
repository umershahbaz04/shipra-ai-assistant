using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.DriverAccountFeatures.Commands.CreateExpense;
public class CreateExpenseCommand : IRequest<ServiceResultDTO>
{
  public string? DriverId { get; set; }
  public string? DriverReceiveableId { get; set; }
  public string? DeliveryNoteId { get; set; }
  public decimal Amount { get; set; }
  public DateTime ExpenseDate { get; set; }
  public int ExpenseCategoryId { get; set; }
  public string? Details { get; set; }
}
