using System;
using System.Collections.Generic;
using System.Text;
using Shipra.Backend.API.Application.DTOs.Common.Base.Response;

namespace Shipra.Backend.API.Application.DTOs.Common.Response;

public class CommunicationRuleResponseDTO : BaseGetResponseDTO
{
  public int Channel { get; set; } // enum property value
  public string? Value { get; set; }
}
