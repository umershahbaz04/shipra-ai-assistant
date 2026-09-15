using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common;
using Shipra.Backend.API.Application.Services.Implementation;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Models;

namespace Shipra.Backend.API.Application.Common.Security;
public class AWSAuthorizationFilter : IAuthorizationFilter
{
  private readonly DynamicPermissionAppService _dynamicPermissionAppService;
  private readonly IAmazonCognitoIdentityProvider _identityProvider;
  private readonly IAuthenticationService _authenticationService;

  public AWSAuthorizationFilter(
      DynamicPermissionAppService dynamicPermissionAppService,
      IAmazonCognitoIdentityProvider identityProvider,
      IAuthenticationService authenticationService)
  {
    _dynamicPermissionAppService = dynamicPermissionAppService;
    _identityProvider = identityProvider;
    _authenticationService = authenticationService;
  }

  public void OnAuthorization(AuthorizationFilterContext context)
  {
    if (IsAuthorizationHeaderAvailable(context.HttpContext.Request))
    {
      ServiceResultDTO serviceResult = GetAccessTokenFromRequest(context.HttpContext.Request);

      if (!serviceResult.IsSuccess)
      {
        context.Result = new UnauthorizedResult();
        return;
      }

      if (serviceResult.Result is AutorizationUserAttributeModel userModel)
      {
        int roleId = userModel.RoleId;

        // Admin has all permissions
        if (roleId != (int)EnumUserRole.Admin)
        {
          var userPermissionList =
              _dynamicPermissionAppService
              .GetGivenPermissionsByUserIdAsync(
                  new ClientId(new Guid(userModel.ClientId!)),
                  roleId)
              .Result;

          var controller = context.ActionDescriptor.RouteValues["controller"]!.ToLower();
          var action = context.ActionDescriptor.RouteValues["action"]!.ToLower();

          // ✅ FIXED LOGIC (single permission check)
          bool hasPermission = userPermissionList.Any(x =>
              x.ControllerName?.ToLower() == controller &&
              x.ActionName?.ToLower() == action &&
              x.GroupAssigned);

          if (!hasPermission)
          {
            serviceResult.CreateError("Authorization",
                            new[] { "You do not have the necessary permissions to access this resource." });
            serviceResult.StatusCode = (int)HttpStatusCode.Forbidden;

            context.Result = new ObjectResult(serviceResult)
            {
              StatusCode = (int)HttpStatusCode.Forbidden
            };
            return;
          }
        }
      }
    }
    else
    {
      context.Result = new UnauthorizedResult();
    }
  }

  private bool IsAuthorizationHeaderAvailable(HttpRequest request)
  {
    return request.Headers.ContainsKey("Authorization");
  }

  private ServiceResultDTO GetAccessTokenFromRequest(HttpRequest request)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    if (IsAuthorizationHeaderAvailable(request))
    {
      var authorizationHeader = request.Headers["Authorization"];
      var handler = new JwtSecurityTokenHandler();
      var valueAuthorizationHeader =
          authorizationHeader.ToString().Replace("Bearer ", string.Empty);

      serviceResult = ValidateToken(valueAuthorizationHeader);

      if (serviceResult.IsSuccess)
      {
        var accessToken = handler.ReadJwtToken(valueAuthorizationHeader);

        foreach (var claim in accessToken.Claims)
        {
          if (claim.Type == "iss")
          {
            // kept as-is
          }
          if (claim.Type == "client_id")
          {
            break;
          }
        }
      }
    }

    return serviceResult;
  }

  private ServiceResultDTO ValidateToken(string accessToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var request = new GetUserRequest
      {
        AccessToken = accessToken
      };

      var response = _identityProvider.GetUserAsync(request).Result;

      if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
      {
        AutorizationUserAttributeModel userObject =
            new AutorizationUserAttributeModel();

        foreach (AttributeType attribute in response.UserAttributes)
        {
          switch (attribute.Name)
          {
            case "username":
              userObject.Username = attribute.Value;
              break;
            case "email":
              userObject.Email = attribute.Value;
              break;
            case "custom:ClientId":
              userObject.ClientId = attribute.Value;
              break;
            case "custom:EmployeeId":
              userObject.EmployeeId = attribute.Value;
              break;
            case "custom:roleId":
              userObject.RoleId = int.Parse(attribute.Value);
              break;
          }
        }

        serviceResult = new ServiceResultDTO(userObject)
        {
          IsSuccess = true
        };
      }
      else
      {
        serviceResult.IsSuccess = false;
      }

      return serviceResult;
    }
    catch (Exception)
    {
      serviceResult.StatusCode = (int)HttpStatusCode.ExpectationFailed;
      serviceResult.IsSuccess = false;
      serviceResult.Errors!.Add(
          "Invalid Token",
          new string[] { "Get New Access Token" });

      return serviceResult;
    }
  }
}
