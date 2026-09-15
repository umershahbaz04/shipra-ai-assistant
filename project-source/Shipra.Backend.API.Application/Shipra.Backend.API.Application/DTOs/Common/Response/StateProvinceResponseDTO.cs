using System;
using System.Collections.Generic;
using System.Text;
using Shipra.Backend.API.Application.DTOs.Common.Base.Response;

namespace Shipra.Backend.API.Application.DTOs.Common.Response;

public class StateProvinceResponseDTO : BaseGetResponseDTO
{
  public string? Name { get; set; }
}
