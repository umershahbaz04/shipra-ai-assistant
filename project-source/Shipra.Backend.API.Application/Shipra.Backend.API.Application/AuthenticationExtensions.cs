using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Services.Implementation;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Models;

namespace Shipra.Backend.API.Application;
public static class AuthenticationExtensions
{
  public static IServiceCollection AddScopedJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddScoped<IAWSCognitoConfigService, AWSCognitoConfigService>();

    services
      .AddAuthentication(options =>
      {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      })
      .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
      {
        // ✅ Static validation params. No per-request mutation.
        options.TokenValidationParameters = new TokenValidationParameters
        {
          ValidateIssuerSigningKey = true,
          ValidateIssuer = true,
          ValidateLifetime = true,
          ValidateAudience = true,
          ClockSkew = TimeSpan.FromMinutes(1),

          // ✅ We validate issuer via IssuerValidator using the token's issuer.
          IssuerValidator = (issuer, securityToken, validationParameters) =>
          {
            // Optional: enforce cognito issuer format
            if (string.IsNullOrWhiteSpace(issuer) ||
                !issuer.StartsWith("https://cognito-idp.", StringComparison.OrdinalIgnoreCase))
            {
              throw new SecurityTokenInvalidIssuerException($"Invalid issuer: {issuer}");
            }
            return issuer; // accept token issuer
          },

          // ✅ Keys resolved per request from token issuer (multi-pool safe)
          IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
          {
            if (securityToken is not JwtSecurityToken jwt)
              return Enumerable.Empty<SecurityKey>();

            var iss = jwt.Issuer;
            if (string.IsNullOrWhiteSpace(iss))
              return Enumerable.Empty<SecurityKey>();

            var jwksUrl = $"{iss}/.well-known/jwks.json";
            return JwksCache.GetKeys(jwksUrl);
          },

          // ✅ Audience validator that supports Cognito access tokens (client_id)
          AudienceValidator = (audiences, securityToken, validationParameters) =>
          {
            if (securityToken is not JwtSecurityToken jwt)
              return false;

            // Cognito access token uses "client_id"
            var tokenClientId = jwt.Claims.FirstOrDefault(c => c.Type == "client_id")?.Value;
            if (string.IsNullOrWhiteSpace(tokenClientId))
              return false;

            // If you have multiple app clients, you can validate via a whitelist.
            // If you validate dynamically per tenant, do it in OnTokenValidated.
            return true;
          }
        };

        options.Events = new JwtBearerEvents
        {
          OnMessageReceived = context =>
          {
            // ✅ Always read Authorization token and set context.Token
            var authHeader = context.Request.Headers["Authorization"].ToString();
            if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
              var accessToken = authHeader.Substring("Bearer ".Length).Trim();
              context.Token = accessToken; // IMPORTANT
            }

            return Task.CompletedTask;
          },

          OnTokenValidated = async context =>
          {
            // ✅ At this point access token is valid (signature/issuer/lifetime)

            // Require x-id-token (your business requirement)
            var idTokenHeader = context.Request.Headers["x-id-token"].ToString();
            if (string.IsNullOrWhiteSpace(idTokenHeader))
            {
              context.Fail("Missing x-id-token.");
              return;
            }

            // Validate x-id-token too (recommended; previously you only decoded it)
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(idTokenHeader))
            {
              context.Fail("Invalid x-id-token format.");
              return;
            }

            var accessJwt = context.SecurityToken as JwtSecurityToken;
            if (accessJwt == null)
            {
              context.Fail("Invalid access token.");
              return;
            }

            // ---- Validate ID token signature using same issuer JWKS ----
            var idJwt = handler.ReadJwtToken(idTokenHeader);

            // Ensure issuer matches access token issuer (same pool)
            if (!string.Equals(idJwt.Issuer, accessJwt.Issuer, StringComparison.Ordinal))
            {
              context.Fail("ID token issuer does not match access token issuer.");
              return;
            }

            // Validate ID token signature/lifetime
            var idValidationParams = new TokenValidationParameters
            {
              ValidateIssuerSigningKey = true,
              ValidateIssuer = true,
              ValidIssuer = idJwt.Issuer,
              ValidateLifetime = true,
              ValidateAudience = true,
              ClockSkew = TimeSpan.FromMinutes(1),

              // ID token uses "aud" as the app client id
              ValidAudience = idJwt.Audiences.FirstOrDefault(),

              IssuerSigningKeyResolver = (token, securityToken, kid, p) =>
              {
                var jwksUrl = $"{idJwt.Issuer}/.well-known/jwks.json";
                return JwksCache.GetKeys(jwksUrl);
              }
            };

            try
            {
              handler.ValidateToken(idTokenHeader, idValidationParams, out _);
            }
            catch (Exception ex)
            {
              context.Fail($"Invalid x-id-token: {ex.Message}");
              return;
            }

            // ---- Extract custom claims from ID token ----
            int roleId = 0;
            var roleClaim = idJwt.Claims.FirstOrDefault(c => c.Type == "custom:roleId")?.Value;
            if (!string.IsNullOrWhiteSpace(roleClaim) && int.TryParse(roleClaim, out var parsedRoleId))
              roleId = parsedRoleId;

            var clientId = idJwt.Claims.FirstOrDefault(c => c.Type == "custom:ClientId")?.Value ?? "";
            var employeeId = idJwt.Claims.FirstOrDefault(c => c.Type == "custom:EmployeeId")?.Value ?? "";

            context.HttpContext.Items["ClientId"] = clientId;
            context.HttpContext.Items["EmployeeId"] = employeeId;
            context.HttpContext.Items["RoleId"] = roleId;

            // username from access token
            var username = accessJwt.Claims.FirstOrDefault(c => c.Type == "username")?.Value;
            if (!string.IsNullOrWhiteSpace(username))
              context.HttpContext.Items["UserName"] = username;

            // ---- Multi-tenant: validate access token client_id against tenant config (optional but strong) ----
            var tokenClientId = accessJwt.Claims.FirstOrDefault(c => c.Type == "client_id")?.Value;
            if (string.IsNullOrWhiteSpace(tokenClientId))
            {
              context.Fail("Missing client_id in access token.");
              return;
            }

            // If you need dynamic validation per tenant, do it HERE (safe, per request)
            var userPoolId = accessJwt.Issuer.Split('/').LastOrDefault();
            if (!string.IsNullOrWhiteSpace(userPoolId))
            {
              var awsConfigService = context.HttpContext.RequestServices.GetRequiredService<IAWSCognitoConfigService>();

              // ✅ Here choose what identifies tenant in your system:
              // - If tenant is custom:ClientId => pass clientId
              // - If tenant is user sub mapping => pass accessJwt.Subject (sub)
              // I'm using custom:ClientId (recommended, since you already store it).
              var awsOptions = await awsConfigService.GetAWSUserClientOptionsAsync(clientId, userPoolId, username ?? "");

              if (!string.Equals(tokenClientId, awsOptions.UserPoolClientId, StringComparison.Ordinal))
              {
                context.Fail("Access token client_id does not match configured tenant app client.");
                return;
              }
            }

            // ---- Permissions (async, avoid .Result) ----
            if (!string.IsNullOrWhiteSpace(clientId))
            {
              var permService = context.HttpContext.RequestServices.GetRequiredService<DynamicPermissionAppService>();

              var permissions = await permService.GetPermissionsForUserAsync(new ClientId(new Guid(clientId)), roleId);

              var routeData = context.HttpContext.GetRouteData();
              var controller = routeData.Values["controller"]?.ToString()?.ToLowerInvariant();
              var action = routeData.Values["action"]?.ToString()?.ToLowerInvariant();

              if (!string.IsNullOrWhiteSpace(controller) && !string.IsNullOrWhiteSpace(action))
              {
                var allowed = permissions.Any(p =>
                  p.ControllerName?.ToLower() == controller &&
                  p.ActionName?.ToLower() == action &&
                  p.GroupAssigned == true);

                //if (!allowed)
                //{
                //  context.HttpContext.Items["AuthorizationFailed"] = true;
                //  context.Fail("You do not have the necessary permissions to access this resource.");
                //}
              }
            }
          },

          OnChallenge = context =>
          {
            context.HandleResponse();

            var isForbidden = context.HttpContext.Items.ContainsKey("AuthorizationFailed");
            context.Response.StatusCode = isForbidden ? StatusCodes.Status403Forbidden : StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var result = new
            {
              type = "https://tools.ietf.org/html/rfc7235#section-3.1",
              title = isForbidden ? "Forbidden" : "Unauthorized",
              status = context.Response.StatusCode,
              traceId = context.HttpContext.TraceIdentifier
            };

            return context.Response.WriteAsync(JsonConvert.SerializeObject(result));
          },

          OnAuthenticationFailed = context =>
          {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
          }
        };
      });

    services.AddAuthorization();
    return services;
  }
}

public static class JwksCache
{
  private static readonly Dictionary<string, List<JsonWebKey>> CachedKeys = new();
  private static readonly Dictionary<string, DateTime> LastFetchTimes = new();
  private static readonly object Lock = new();

  public static IEnumerable<SecurityKey> GetKeys(string jwksUrl)
  {
    lock (Lock)
    {
      if (CachedKeys.TryGetValue(jwksUrl, out var cached) &&
          LastFetchTimes.TryGetValue(jwksUrl, out var last) &&
          (DateTime.UtcNow - last).TotalHours <= 24)
      {
        return cached;
      }

      using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };

      Exception? lastEx = null;
      for (int attempt = 1; attempt <= 3; attempt++)
      {
        try
        {
          var jwks = httpClient.GetStringAsync(jwksUrl).GetAwaiter().GetResult();
          var keys = new JsonWebKeySet(jwks).Keys.ToList();

          CachedKeys[jwksUrl] = keys;
          LastFetchTimes[jwksUrl] = DateTime.UtcNow;
          return keys;
        }
        catch (Exception ex)
        {
          lastEx = ex;
          // small backoff
          Thread.Sleep(200 * attempt);
        }
      }

      throw new Exception($"Failed to download JWKS from {jwksUrl}", lastEx);
    }
  }

}
