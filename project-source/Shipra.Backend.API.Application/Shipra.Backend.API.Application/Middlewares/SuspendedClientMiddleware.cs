using MathNet.Numerics.RootFinding;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Models;

namespace Shipra.Backend.API.Application.Middlewares;

public class SuspendedClientMiddleware
{
  private readonly RequestDelegate _next;

  public SuspendedClientMiddleware(RequestDelegate next)
  {
    _next = next;
  }

  //public async Task InvokeAsync(HttpContext context)
  //{
  //  var tenantProvider = context.RequestServices.GetRequiredService<ICurrentTenantService>();
  //  var tenant = tenantProvider.GetTenantAsyncWithOperation(context);

  //  if (tenant is not null && tenant.Catalogue is not null && tenant.CatalogueDatabase is not null)
  //  {
  //    //check if any catalog and catalogdatabse is not active
  //    if (!CheckActiveStatus(tenant,context))
  //    {
  //      ErrorResponse errorResponse = CreateErrorResponse(tenant);

  //      context.Response.StatusCode = StatusCodes.Status403Forbidden;
  //      context.Response.ContentType = "application/json";
  //      // Return the error message as part of the API response
  //      // Return the error response as part of the API response
  //      var jsonResponse = JsonConvert.SerializeObject(new { error = errorResponse });
  //      await context.Response.WriteAsync(jsonResponse);
  //      return;
  //    }

  //  }

  //  await _next(context);
  //}

  public async Task InvokeAsync(HttpContext context)
  {
    var tenantProvider = context.RequestServices.GetRequiredService<ICurrentTenantService>();
    OperationStatusResponseModel? tenant;

    try
    {
      tenant = tenantProvider.GetTenantAsyncWithOperation(context);
    } 
    catch (Exception ex)
    {

      if (ex.Message.Contains("Access Token has expired"))
      { 
        // Handle token expiration or AuthenticationTimeout access
        context.Response.StatusCode = StatusCodes.Status419AuthenticationTimeout;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonConvert.SerializeObject(new
        {
          error = "TokenExpired",
          message = "Your session has expired. Please log in again."
        }));
        return;
      }
      else
      {
        // Log the exception (ensure ILogger is injected in the middleware)
        //_logger.LogError(ex, "An error occurred while retrieving the connection string.");
        _ = ex.Message;
        // Return a user-friendly error response
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonConvert.SerializeObject(new
        {
          error = "Internal Server Error",
          message = "An unexpected error occurred while processing your request. Please try again later."
        }));
        return;
      } 
    }

    if (tenant is not null && tenant.Catalogue is not null && tenant.CatalogueDatabase is not null)
    {
      if (!CheckActiveStatus(tenant, context))
      {
        ErrorResponse errorResponse = CreateErrorResponse(tenant);

        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        context.Response.ContentType = "application/json";

        var jsonResponse = JsonConvert.SerializeObject(new { error = errorResponse });
        await context.Response.WriteAsync(jsonResponse);
        return;
      }
    }

    await _next(context);
  }

  public string GetEndpointName(HttpContext context)
  {
    // Get the requested path
    var path = context.Request.Path;
    // Split the path into segments and take the last segment
    var segments = path!.Value!.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
    if (segments.Length > 0)
    {
      return segments.Last(); // Get the last segment
    }

    return string.Empty; // Return an empty string if no segments
  }
  private bool CheckActiveStatus(OperationStatusResponseModel tenant, HttpContext context)
  {
    var endpointName = GetEndpointName(context);


    bool isActive = true;
    ////skip some query and command weather user or database is not active
    if (!ApplicationConstants.SubscriptionSkipCommandQueryList.Any(cmd => cmd.ToLower() == endpointName.ToLower()))
    {
      if (tenant.Catalogue!.OperationalStatusId.GetValueOrDefault((int)EnumOperationalStatus.Active) != (int)EnumOperationalStatus.Active)
      {
        isActive = false;
      }
      if (tenant.CatalogueDatabase!.OperationalStatusId.GetValueOrDefault((int)EnumOperationalStatus.Active) != (int)EnumOperationalStatus.Active)
      {
        isActive = false;
      }
    }
    return isActive;
  }

  private ErrorResponse CreateErrorResponse(OperationStatusResponseModel tenant)
  {
    int operationalStatusId = (int)EnumOperationalStatus.Active;

    //first priority database deployemenc check 

    if (tenant.CatalogueDatabase is not null)
    {
      operationalStatusId = tenant.CatalogueDatabase.OperationalStatusId.GetValueOrDefault((int)EnumOperationalStatus.Active);
    }
    // get user operational status
    else if (tenant.Catalogue is not null)
    {
      operationalStatusId = tenant.Catalogue.OperationalStatusId.GetValueOrDefault((int)EnumOperationalStatus.Active);
    }
    var errorResponse = new ErrorResponse
    {
      operationalStaus = operationalStatusId,
      errorMessage = "Client account is suspended.",
      errorDetails = "Your account has been suspended due to some reason contact with admin."
    };

    return errorResponse;
  }
}
public class ErrorResponse
{
  public int operationalStaus { get; set; }
  public string? errorMessage { get; set; }
  public string? errorDetails { get; set; }
}

