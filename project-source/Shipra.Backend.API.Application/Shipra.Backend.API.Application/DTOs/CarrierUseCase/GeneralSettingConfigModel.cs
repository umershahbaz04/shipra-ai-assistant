using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Application.DTOs.CarrierUseCase;
  
public class CarrierSettingsConfig
{
  public string? AccountNumber { get; set; }
  public string? UserName { get; set; }
  public string? Password { get; set; }
  public string? DomainProdURL { get; set; }
  public string? DomainTestURL { get; set; }
}
