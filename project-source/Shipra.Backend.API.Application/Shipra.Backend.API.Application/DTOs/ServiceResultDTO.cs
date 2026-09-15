using System.Net;
using Shipra.Backend.API.Application.DTOs.Common.Base.Response;

namespace Shipra.Backend.API.Application.DTOs;
public class ServiceResultDTOWithTypeModel<T> : ErrorResponse
{
  public T? Result { get; protected set; }

  public ServiceResultDTOWithTypeModel()
  {
  }

  public ServiceResultDTOWithTypeModel(T result)
  {
    Result = result;
  }
}

public class ServiceResultDTO : ErrorResponse
{
  public dynamic? Result { get; protected set; }

  public ServiceResultDTO()
  {
  }

  public ServiceResultDTO(dynamic result, bool isSuccess = true)
  {
    Result = result;
    IsSuccess = isSuccess;
    StatusCode = (int)HttpStatusCode.OK;
  }
}
