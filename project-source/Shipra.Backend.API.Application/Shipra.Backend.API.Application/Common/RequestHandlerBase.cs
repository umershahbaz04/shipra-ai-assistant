using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using AutoMapper;
using DocumentFormat.OpenXml.InkML;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common.Context.Models;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.DTOs.Common.Base.Response;
using Shipra.Backend.API.Application.Services;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Models;

namespace Shipra.Backend.API.Application.Common;


public abstract class RequestHandlerBase
{

  private readonly IServiceProvider _serviceProvider;
  /// <summary>
  /// Gets the mapper.
  /// </summary>
  /// <value>
  /// The mapper.
  /// </value>
  protected IMapper _mapper;
  private IExceptionHelper _exceptionHelper;
  public IHttpContextAccessor _httpContextAccessor;
  public ExceptionHandlerService _exceptionHandlerService;
  public IEmailServiceProvider _emailServiceProvider;
  protected ILogger Logger { get; }
  protected IAmazonCognitoIdentityProvider _identityProvider;
  protected IConfiguration _configuration; 

  protected RequestHandlerBase(IServiceProvider serviceProvider, ILogger logger)
  {
    _serviceProvider = serviceProvider;
    Logger = logger;
    _exceptionHelper = _serviceProvider.GetRequiredService<IExceptionHelper>();
    _mapper = _serviceProvider.GetRequiredService<IMapper>();
    _httpContextAccessor = _serviceProvider.GetRequiredService<IHttpContextAccessor>();
    _identityProvider = _serviceProvider.GetRequiredService<IAmazonCognitoIdentityProvider>();
    _exceptionHandlerService = _serviceProvider.GetRequiredService<ExceptionHandlerService>();
    _configuration = _serviceProvider.GetRequiredService<IConfiguration>();
    _emailServiceProvider = _serviceProvider.GetRequiredService<IEmailServiceProvider>();
     
  }


  protected async Task<TResponseType> RunAsync<TRequestType, TResponseType>(
      TRequestType requestBody,
      Func<Task<TResponseType>> action)
      where TResponseType : IErrorResponse, new()
  {
    var result = new TResponseType(); 
     
    //var requstObject = typeof(TRequestType).GetProperty("SendEmail"); // looks dirty to me

    try
    {

      var requestType = $"{typeof(TRequestType).ToString() ?? "null"}";
      Logger.LogTrace("Processing request '{requestType}'", requestType);
      result = await action();
      return result;
    }
    catch (Exception ex)
    {
      ErrorResponse? response = null;
      string tenantId = GetClientIdFromRequest()!;
      var emailBody = new StringBuilder();
       

      // Handle the exception and build the email body
      if (ex is ShipraApplicationException || ex is NotSupportedException)
      {
        response = _exceptionHelper.GetErrorResponse(ex, tenantId, sendEmail: false, customMessage: ex.Message, logToDb: false);
      }
      else
      {
        Logger.LogError($"{tenantId} {ex.Message} {ex.InnerException} {ex.StackTrace}");
        response = _exceptionHelper.GetErrorResponse(ex, tenantId, sendEmail: false, ex.Message);

        // Build the email body
        emailBody.AppendLine("<h3>An error occurred in the application</h3>");
        // Serialize the request body
        var serializedRequestBody = string.Empty;
        try
        {
          serializedRequestBody = Newtonsoft.Json.JsonConvert.SerializeObject(
              requestBody,
              Newtonsoft.Json.Formatting.Indented,
              new Newtonsoft.Json.JsonSerializerSettings
              {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
              });
          emailBody.AppendLine($"<p><strong>Request Body:</strong></p><pre>{serializedRequestBody}</pre>");
        }
        catch (Exception serializationEx)
        {
          Logger.LogError(serializationEx, "Failed to serialize the request body.");
          emailBody.AppendLine($"<p><strong>Request Body:</strong> Could not serialize due to an error.</p>");
        }

        _exceptionHandlerService.SendEmail(tenantId, serializedRequestBody, ex);
      }

      result.IsSuccess = false;
      result.Errors = response.Errors;

      return result;
    }

  }

  private string? GetClientIdFromRequest()
  {
    StringBuilder builder = new StringBuilder();
    string? clientId = string.Empty;
    string? userName = string.Empty;
    #region check token from header and get userid
    if (_httpContextAccessor.HttpContext!.Request.Headers.ContainsKey("Authorization"))
    {
      var authorizationHeader = _httpContextAccessor.HttpContext!.Request.Headers["Authorization"];
      var handler = new JwtSecurityTokenHandler();
      var valueAuthorizationHeader = authorizationHeader.ToString().Replace("Bearer ", string.Empty);
      if (!string.IsNullOrEmpty(valueAuthorizationHeader))
      {
        var accessToken = handler.ReadJwtToken(valueAuthorizationHeader);

        #region extract values from context
        try
        {
          // Try to get values from context items
          clientId = _httpContextAccessor.HttpContext!.Items["ClientId"]?.ToString();

          // Fallback to form data if ClientId is null
          if (string.IsNullOrEmpty(clientId) && _httpContextAccessor.HttpContext.Request.HasFormContentType)
          {
            clientId = _httpContextAccessor.HttpContext.Request.Form["ClientId"];
          }

          // Try to get other values from context items or form
          userName = _httpContextAccessor.HttpContext!.Items["UserName"]?.ToString()
                                  ?? _httpContextAccessor.HttpContext.Request.Form["UserName"];


        }
        catch (Exception ex)
        {
          _ = ex.Message;
        }
        #endregion 
      }

    }
    if (_httpContextAccessor.HttpContext!.Request.Headers.ContainsKey("X-TenantId"))
    {
      clientId = _httpContextAccessor.HttpContext!.Request.Headers["X-TenantId"];
    }
    #endregion 
    if (!string.IsNullOrEmpty(clientId))
    {
      builder.Append(clientId);
    }
    if (!string.IsNullOrEmpty(userName))
    {
      if (builder.Length > 0)
      {
        builder.Append(" - "); // Separator between clientId and userName
      }
      builder.Append(userName);
    }
    var clientStr = builder.Length > 0 ? builder.ToString() : "Client not found";

    return clientStr;
  }
}

/// <summary>
/// Middleware for processing MediatR commands.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
/// <seealso cref="MediatR.IRequestHandler{TRequest, TResponse}" />
public abstract class RequestHandlerBase<TRequest, TResponse> : RequestHandlerBase, IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse> where TResponse : IErrorResponse, new()
{
  private ClaimsIdentity? _identity;
  public IEnumerable<Claim>? _claims { get; }
  public CurrentUserContext _currentUser = new CurrentUserContext();
  protected RequestHandlerBase(IServiceProvider serviceProvider, ILogger logger) : base(serviceProvider, logger)
  {
    // Resolve IConfiguration from IServiceProvider
    _configuration = serviceProvider.GetRequiredService<IConfiguration>();
    _identity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
    _claims = _identity!.Claims;
    _currentUser.EnvironmentTypeId = Convert.ToInt32(_configuration["EnvironmentTypeId"]);
    //_currentUser.ClientIdStr = _claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault()?.Value;
    //_currentUser.UserName = _claims?.Where(x => x.Type == ClaimTypes.Email).FirstOrDefault()?.Value;

    //hardcode clientId

    #region check token from header and get userid
    if (_httpContextAccessor.HttpContext!.Request.Headers.ContainsKey("Authorization"))
    {
      var authorizationHeader = _httpContextAccessor.HttpContext!.Request.Headers["Authorization"];
      var handler = new JwtSecurityTokenHandler();
      var valueAuthorizationHeader = authorizationHeader.ToString().Replace("Bearer ", string.Empty);
      if (!string.IsNullOrEmpty(valueAuthorizationHeader))
      {
        var accessToken = handler.ReadJwtToken(valueAuthorizationHeader);

        _currentUser.AccessToken = valueAuthorizationHeader;

        #region extract values from context
        try
        {
          // Try to get values from context items
          _currentUser.ClientIdStr = _httpContextAccessor.HttpContext!.Items["ClientId"]?.ToString();
          _currentUser.RoleId = (int?)_httpContextAccessor.HttpContext!.Items["RoleId"];

          // Fallback to form data if ClientId is null
          if (string.IsNullOrEmpty(_currentUser.ClientIdStr) && _httpContextAccessor.HttpContext.Request.HasFormContentType)
          {
            _currentUser.ClientIdStr = _httpContextAccessor.HttpContext.Request.Form["ClientId"];
          }

          if (!string.IsNullOrEmpty(_currentUser.ClientIdStr))
          {
            _currentUser.ClientId = new ClientId(new Guid(_currentUser.ClientIdStr!));
          }
          if (_currentUser.RoleId == 0)
          {
            _currentUser.RoleId = int.Parse(_httpContextAccessor.HttpContext.Request.Form["RoleId"]!);
          }
          _currentUser.EmployeeIdStr = _httpContextAccessor.HttpContext!.Items["EmployeeId"]?.ToString();

          if (string.IsNullOrEmpty(_currentUser.EmployeeIdStr) && _httpContextAccessor.HttpContext.Request.HasFormContentType)
          {
            _currentUser.EmployeeIdStr = _httpContextAccessor.HttpContext.Request.Form["EmployeeId"];
          }
          if (!string.IsNullOrEmpty(_currentUser.EmployeeIdStr))
          {
            _currentUser.EmployeeId = new EmployeeId(new Guid(_currentUser.EmployeeIdStr!));
          }

          // Try to get other values from context items or form
          _currentUser.UserName = _httpContextAccessor.HttpContext!.Items["UserName"]?.ToString()
                                  ?? _httpContextAccessor.HttpContext.Request.Form["UserName"];


        }
        catch (Exception ex)
        {
          _ = ex.Message;
        }
        #endregion 
      }

    }
    if (_httpContextAccessor.HttpContext!.Request.Headers.ContainsKey("X-TenantId"))
    {
      var clientId = _httpContextAccessor.HttpContext!.Request.Headers["X-TenantId"];
      _currentUser.ClientIdStr = clientId;
      _currentUser.ClientId = new ClientId(new Guid(_currentUser.ClientIdStr!));

      _currentUser.EmployeeIdStr = _currentUser.ClientIdStr;
      _currentUser.EmployeeId = new EmployeeId(new Guid(_currentUser.EmployeeIdStr!));
    }
    #endregion
    foreach (var item in _claims!.Where(x => x.Type == ClaimTypes.Role).ToList())
    {
      var role = item.Value;
      _currentUser.NotMappedRoles.Add(role);
    }
  }

  public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
  {
    if (request == null)
    {
      throw new ShipraApplicationException(System.Net.HttpStatusCode.BadRequest,
          $"{typeof(TRequest)} was null.",
          new ArgumentNullException(typeof(TRequest).ToString()));
    }

    // Pass the request to RunAsync
    var result = await RunAsync<TRequest, TResponse>(request, async () => await HandleRequest(request, cancellationToken));
    return result;
  }

  /// <summary>
  /// Handles the request.
  /// </summary>
  /// <param name="request">The request.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns></returns>
  protected abstract Task<TResponse> HandleRequest(TRequest request, CancellationToken cancellationToken);

}
