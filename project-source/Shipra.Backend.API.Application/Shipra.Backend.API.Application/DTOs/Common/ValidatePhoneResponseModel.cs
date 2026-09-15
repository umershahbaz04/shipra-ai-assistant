using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Application.DTOs.Common;
public class ValidatePhoneResponseModel
{
  public bool? IsValid { get; set; } = true;
  public string? PhoneNumber { get; set; }
}
