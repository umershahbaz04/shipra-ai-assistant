using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.DTOs.OrderUseCase.HelperOrder.UDTOrder;

public class UDTOrderDetail
{
  public int UDTOrderDetailID { get; set; }
  public decimal? Amount { get; set; }
}
