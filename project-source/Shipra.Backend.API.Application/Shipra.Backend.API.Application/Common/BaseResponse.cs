using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Application.DTOs.Common.Base.Response;

namespace Shipra.Backend.API.Application.Common;

public class BaseResponse<T>  
{
  public BaseResponse()
  {
    Errors = new Dictionary<string, string[]>();
  }

  public bool HasError => Errors?.Count() > 0;
  public bool IsSuccess { get; set; } = false;
  public string? Message { get; set; }
  public Dictionary<string, string[]>? Errors { get; set; }
  public int Total { get; set; }
  public List<T>? Result { get; set; }
  public int Code { get; set; } 
}
