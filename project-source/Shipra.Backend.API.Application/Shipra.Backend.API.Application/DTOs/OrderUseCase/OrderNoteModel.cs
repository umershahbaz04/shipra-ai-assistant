using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Application.DTOs.OrderUseCase;
public class OrderNoteModel
{
  public string? OrderNoteId { get; set; }
  public string? Note { get; set; }
}
public class GeneralOrderAddressModel
{
  public OrderAddressModel? OrderAddress { get; set; } = new(); 
}
