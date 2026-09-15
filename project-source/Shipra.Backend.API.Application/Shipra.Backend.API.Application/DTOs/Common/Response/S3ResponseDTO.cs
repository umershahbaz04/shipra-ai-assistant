using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Application.DTOs.Common.Response;
public class S3ResponseDTO
{ 
  public string Message { get; set; } = string.Empty;
  public string Url { get; set; } = string.Empty;

  public void CreateS3ResponseDTO(dynamic response)
  { 
    var type = response.GetType();
    Message = (string)type.GetProperty("Message").GetValue(response); 
    Url = (string)type.GetProperty("Url").GetValue(response);
  }
}
