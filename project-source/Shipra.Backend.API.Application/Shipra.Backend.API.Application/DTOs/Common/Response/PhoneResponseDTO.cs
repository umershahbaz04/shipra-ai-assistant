using System;
using System.Collections.Generic;
using System.Text;
using Shipra.Backend.API.Application.DTOs.Common.Base.Response;

namespace Shipra.Backend.API.Application.DTOs.Common.Response;

public class PhoneResponseDTO : BaseGetResponseDTO
{
  public string? FullNumber { get; set; }
  public int PhoneType { get; set; }
  public Guid? PatientId { get; set; }
  public Guid? ProviderId { get; set; }
}
