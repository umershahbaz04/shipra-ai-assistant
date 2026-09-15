using System;

namespace Shipra.Backend.API.Application.DTOs.Common.Base.Response;

public class BaseResponseDTO : ErrorResponse
{
  public Guid Id { get; set; }
  public int CreatedById { get; set; }
  public int ModifiedById { get; set; }

  public DateTime CreatedDate { get; set; }
  public DateTime? ModifiedDate { get; set; }

  public bool IsActive { get; set; }
}
