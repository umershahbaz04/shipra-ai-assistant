using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Application.DTOs.OrderLabelUseCase;
public class CreateOrderLabelModel
{
  public string? Label { get; set; }
  public string? ColorCode { get; set; }

}
