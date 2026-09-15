using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Microsoft.AspNetCore.Http;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;

namespace Shipra.Backend.API.Application.Services.Implementation;
public class CurrentTenantService : ICurrentTenantService
{
  //private readonly ICurrentTenantRepository _currentTenantProviderRepository;
  //private readonly TenantConnectionStringProvider _tenantConnectionStringProvider;
  private readonly ICurrentTenantRepository _currentTenantService;
  private readonly IAmazonCognitoIdentityProvider _identityProvider; 
  private readonly ExceptionHandlerService _exceptionHandlerService;

  public CurrentTenantService(ExceptionHandlerService exceptionHandlerService, IAmazonCognitoIdentityProvider identityProvider, ICurrentTenantRepository currentTenantService)
  {
    _currentTenantService = currentTenantService;
    _exceptionHandlerService = exceptionHandlerService; 
    _identityProvider = identityProvider;
    //_currentTenantProviderRepository = currentTenantProviderRepository;  

  }
  public string GetConnectionStringByTenant(string clientId)
  {
    string connString = string.Empty;
    try
    {
      if (!string.IsNullOrEmpty(clientId))
      {
        connString = _currentTenantService.GetConnectionStringByTenant(clientId);
      }
    }
    catch (Exception ex)
    {
      _ = ex.Message;
      throw new Exception("Invalid Connection please connect with administrator");
    }
    return connString;
  }

  #region operational status
  public OperationStatusResponseModel GetTenantAsyncWithOperation(HttpContext context)
  {
    return GetConnectionString(context);

  }
  #endregion
  public string GetTenantAsync(HttpContext context)
  {
    OperationStatusResponseModel? operationStatus = null;
    try
    {
      operationStatus = GetConnectionString(context);
      return operationStatus!.ConnectionString! ?? string.Empty;
    }
    catch (Exception)
    {
      // send email
      return operationStatus!.ConnectionString! ?? string.Empty; ;
    }


  }
  public OperationStatusResponseModel GetConnectionString(HttpContext context)
  {
    OperationStatusResponseModel operationStatus = new();
    string clientId = string.Empty;
    if (context != null)
    {

      try
      {
        string authorizationHeader = context.Request.Headers["Authorization"]!;
        #region if request contain auth token
        if (!string.IsNullOrEmpty(authorizationHeader))
        {

          var handler = new JwtSecurityTokenHandler();
          var valueAuthorizationHeader = authorizationHeader.ToString().Replace("Bearer ", string.Empty);
          if (!string.IsNullOrEmpty(valueAuthorizationHeader))
          {
            var accessToken = handler.ReadJwtToken(valueAuthorizationHeader);

            //_currentUser.AccessToken = valueAuthorizationHeader;


            #region get custom attribute with access token
            var request = new GetUserRequest
            {
              AccessToken = valueAuthorizationHeader
            };
            try
            {
              var response = _identityProvider.GetUserAsync(request).Result;

              if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
              {
                #region get and set value from context
                int roleId = 0;
                var chkRole = response.UserAttributes.FirstOrDefault(attr => attr.Name == "custom:roleId");
                if (chkRole != null)
                {
                  roleId = !string.IsNullOrEmpty(chkRole?.Value) ? Int32.Parse(chkRole.Value) : 0;
                }
                //var clientAttribute = response.UserAttributes.FirstOrDefault(attr => attr.Name == "custom:ClientId");
                //var employeeAttribute = response.UserAttributes.FirstOrDefault(attr => attr.Name == "custom:EmployeeId");



                int RoleId = roleId;

                // Extract ClientId and EmployeeId
                var clientAttribute = response.UserAttributes.FirstOrDefault(attr => attr.Name == "custom:ClientId");
                var employeeAttribute = response.UserAttributes.FirstOrDefault(attr => attr.Name == "custom:EmployeeId");

                clientId = clientAttribute?.Value ?? string.Empty;
                string employeeId = employeeAttribute?.Value ?? string.Empty;

                // Populate context.Items
                context.Items["ClientId"] = clientId;
                context.Items["EmployeeId"] = employeeId;
                context.Items["RoleId"] = roleId;

                #endregion
                //var clientId = !string.IsNullOrEmpty(clientAttribute?.Value) ? clientAttribute.Value : string.Empty;

                #region Add to Request.Form
                if (context.Request.HasFormContentType)
                {
                  // Preserve the files from the original form
                  var files = context.Request.Form.Files;
                  var formFields = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>(context.Request.Form);

                  // Add new values
                  formFields["ClientId"] = clientId;
                  formFields["EmployeeId"] = employeeId;
                  formFields["RoleId"] = roleId.ToString();

                  // Create a new FormCollection with the updated fields and existing files
                  var updatedForm = new FormCollection(formFields, files);

                  // Update Request.Form with the new collection
                  context.Request.Form = updatedForm;
                }
                #endregion

                // Replace with your own logic to get the connection string based on the client ID
                operationStatus = _currentTenantService.GetConnectionStringByTenantWithOperation(clientId);

                if (string.IsNullOrEmpty(operationStatus.ConnectionString))
                {
                  throw new Exception("No database connection found.");
                }

              }
            }
            catch (Exception ex)
            {
              //GetClientIdFromRequest(clientId,"");
              _ = ex.Message; 
              //return operationStatus;
            }
            #endregion
          }


        }
        else
        {
          #region in case of notification project request recieved 
          string? tenantId = context.Request.Headers["X-TenantId"];
          if (!string.IsNullOrEmpty(tenantId))
          {
            clientId = tenantId;

            Guid clientIdGuid;
            bool isValid = Guid.TryParse(tenantId, out clientIdGuid);
            if (isValid)
            {
              operationStatus.ConnectionString = _currentTenantService.GetConnectionStringByTenant(tenantId!);
              if (string.IsNullOrEmpty(operationStatus.ConnectionString))
              {
                throw new Exception("No database connection found.");
              }
            }
          }
          #endregion
        }
        #endregion
      }
      catch (Exception ex)
      {
        var reqBody = _exceptionHandlerService.GetRequestBodyAsync(context).GetAwaiter().GetResult();
        string? clientInfo = _exceptionHandlerService.GetClientIdFromRequest(clientId, "");
        _exceptionHandlerService.SendEmail(clientInfo, reqBody, ex);
        return operationStatus;
      }
    }
    return operationStatus;

  }

  public string? GetQueryStringValue(string key, HttpContext context)
  {
    if (context != null)
    {

      if (context.Request.Query.ContainsKey(key))
      {
        return context.Request.Query[key].ToString();
      }
      else
      {
        return null; // Or throw an exception, depending on your requirements
      }
    }
    else
    {
      return null; // Or throw an exception, depending on your requirements
    }

  }

}
