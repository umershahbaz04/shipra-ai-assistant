using System;
using System.Collections.Generic;
using System.Text;

namespace Shipra.Backend.API.Application.DTOs.Common.Request;

public class PhoneRequestDTO : BaseCreateRequestDto
{
  public string? FullNumber { get; set; }
  public int PhoneType { get; set; }
  public Guid? PatientId { get; set; }
  public Guid? ProviderId { get; set; }
}
