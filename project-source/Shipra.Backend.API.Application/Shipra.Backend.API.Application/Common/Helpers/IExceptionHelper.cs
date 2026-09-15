using System;
using Shipra.Backend.API.Application.DTOs.Common.Base.Response;

namespace Shipra.Backend.API.Application.Common.Helpers;

public interface IExceptionHelper
{
  ErrorResponse GetErrorResponse(Exception ex, string tenantId, bool sendEmail = false, string? customMessage = null, bool logToDb = true);
  int Log(Exception e, bool sendEmail = false);
}
