using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Shipra.Backend.API.Application.DTOs.SmsUseCase.Smart;
public class SmartSmsGateway
{
  public SmartJsonapi? jsonapi { get; set; }
  public SmartData? data { get; set; }
  public Error[]? errors { get; set; } 
}

public class SmartJsonapi
{
  public string? version { get; set; }
}

public class SmartData
{
  public string? status { get; set; }
  public string? id { get; set; }
}
public class SmartError
{
  public int code { get; set; }
  public string? title { get; set; }
  public string? detail { get; set; }
}
