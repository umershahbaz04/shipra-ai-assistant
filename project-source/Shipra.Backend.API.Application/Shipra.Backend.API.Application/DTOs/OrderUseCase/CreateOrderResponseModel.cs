using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Application.DTOs.OrderUseCase;
public class CreateOrderResponseModel
{
  public List<CreateOrderResponseDetailModel>? data { get; set; }
}
public class CreateOrderResponseDetailModel
{
  public string? OrderId { get; set; }
  public string? OrderNo { get; set; }
  public string? RefNo { get; set; }
  public bool? IsSuccess { get; set; } = false;
  public bool? IsNewCreated { get; set; } = true;
}
