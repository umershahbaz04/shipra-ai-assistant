using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Application.DTOs.CarrierUseCase;
public class UpdateCarrierBackgroundColorRequestModel
{
  public int CarrierId { get; set; }
  public string? BackgroundColor { get; set; }
  public string? BorderColor { get; set; }
}
