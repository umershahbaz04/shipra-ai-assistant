using System;
using System.Collections.Generic;
using System.Text;

namespace Shipra.Backend.API.Application.DTOs.Common.Request;

public class CommunicationRuleRequestDTO : BaseCreateRequestDto
{
  public int Channel { get; set; } // enum property value
  public string? Value { get; set; }
}
