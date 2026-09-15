using System.Net;
using Shipra.Backend.API.Application.Common.Exceptions;

namespace Shipra.Backend.API.Application.DTOs.Common.Base.Response;

public class ErrorResponse : IErrorResponse
{
  public ErrorResponse()
  {
    Errors = new Dictionary<string, string[]>();
    ErrorCombined = new Dictionary<int, string>();

    //PrivateErrors = new List<string>();
    IsSuccess = true;
  }
  public bool IsSuccess { get; set; }

  /// <summary>
  /// PublicErrors for end users
  /// </summary>
  public Dictionary<string, string[]>? Errors { get; set; }
  public Dictionary<string, string[]>? ConfigErrors { get; set; } 
  public Dictionary<int, string>? ErrorCombined { get; set; }
  /// 
  //public string? Error { get; set; }
  /// <summary>
  /// PrivateErrors that are api specifc
  /// </summary>
  //public List<string> PrivateErrors { get; set; }
  //public Exception? OriginalException { get; set; }
  //public string? CustomErrorMessage { get; set; }
  public int ErrorID { get; set; }
  public int StatusCode { get; set; }

  public void CreateError(string key, string[] strings)
  {
    Errors?.Add(key, strings);
    IsSuccess = false;
  }
  public void CreateSuccessResponse(HttpStatusCode statusCode = HttpStatusCode.OK)
  {
    StatusCode = (int)statusCode;
    IsSuccess = true;
  }
  public void CreateConfigError(string key, string[] strings)
  {
    ConfigErrors?.Add(key, strings);
    IsSuccess = false;
  }
  public void CreateErrorResponse(HttpStatusCode statusCode = HttpStatusCode.BadRequest)
  {
    StatusCode = (int)statusCode;
    IsSuccess = false;
  }
  public void CreateErrorResponse(Exception? ex)
  {
    HttpStatusCode? statusCode = HttpStatusCode.ExpectationFailed;
    if (ex is EntityNotFoundException)
    {
      statusCode = HttpStatusCode.NotFound;
    }
    else
    {
    }
    string msg = ex?.Message!;
    if (ex != null && ex!.Message!.Contains("inner exception"))
    {
      msg = ex?.InnerException!.Message!;
    }
    StatusCode = (int)statusCode;
    IsSuccess = false;
    Errors!.Add(ex?.GetType().Name!, new string[] { ex?.Message! });
  }
}
