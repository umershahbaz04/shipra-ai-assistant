using Shipra.Backend.API.Application.DTOs.Common.Base.Response;

namespace Shipra.Backend.API.Application.DTOs.CommonUseCase.Response;
public class CommonLookupDTO<T> : ErrorResponse
{
  public T? Result { get; set; }
}
public class CommonLookupResponseModel
{
  public int id { get; set; }
  public string? text { get; set; }
}
