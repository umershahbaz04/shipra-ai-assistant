using System;
using System.Collections.Generic;
using System.Text;

namespace Shipra.Backend.API.Application.DTOs.Common.Base.Request;

public class BaseUpdateRequestDto : BaseRequestDto
{
  public BaseUpdateRequestDto()
  {
    ModifiedDate = DateTime.UtcNow;
    IsActive = true;
  }

  public DateTime ModifiedDate { get; set; }
  public bool IsActive { get; set; }
}
