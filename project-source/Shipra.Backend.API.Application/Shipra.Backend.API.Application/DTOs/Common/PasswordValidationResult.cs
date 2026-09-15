using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Application.DTOs.Common;

public class PasswordValidationResult
{
  public bool IsValid { get; set; }
  public List<string> Errors { get; set; } = new();
}
