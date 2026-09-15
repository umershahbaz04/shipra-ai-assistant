using System;
using System.Net;

namespace Shipra.Backend.API.Application.Common.Exceptions;

public class ShipraApplicationException : Exception
{
  private readonly HttpStatusCode statusCode;

  public ShipraApplicationException(HttpStatusCode statusCode, string message, Exception ex)
      : base(message, ex)
  {
    this.statusCode = statusCode;
  }

  public ShipraApplicationException(HttpStatusCode statusCode, string message)
      : base(message)
  {
    this.statusCode = statusCode;
  }

  public ShipraApplicationException(HttpStatusCode statusCode)
  {
    this.statusCode = statusCode;
  }

  public HttpStatusCode StatusCode
  {
    get { return statusCode; }
  }
}
