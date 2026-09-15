using System.Collections.Generic;

namespace Shipra.Backend.API.Application.DTOs.Common.Base.Response;

public interface IErrorResponse
{
  public bool IsSuccess { get; set; }
  public Dictionary<string, string[]>? Errors { get; set; }
  //public string? Error { get; set; }

} 
