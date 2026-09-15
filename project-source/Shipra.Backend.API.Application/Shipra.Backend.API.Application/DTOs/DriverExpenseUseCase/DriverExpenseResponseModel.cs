using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.DeliveryNoteAggregate;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Application.DTOs.DriverExpenseUseCase;
public class DriverExpenseResponseModel
{
  public string? ExpenseId { get;  set; } 
  public string? DriverId { get;  set; }
  public string? DriverReceivableId { get;  set; } 
  public decimal Amount { get;  set; }
  public DateTime ExpenseDate { get;  set; }
  public int ExpenseCategoryId { get;  set; }
  public string? Details { get;  set; } 
  public bool? Active { get;  set; }
}
