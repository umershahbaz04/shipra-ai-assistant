namespace Shipra.Backend.API.Application.DTOs;
public class BaseResponseDto
{
  public dynamic? Data { get; set; }
  public string? Message { get; set; }
}

public class GenralResponseDto
{
  public object? result { get; set; }
}
