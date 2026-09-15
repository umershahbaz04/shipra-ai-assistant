using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Infrastructure.Services.Noon;

public static class NoonAuthHelper
{
    /// <summary>
    /// Generates a signed JWT for Noon API authentication using the provided credentials.
    /// </summary>
    public static string GenerateJwtToken(string privateKeyPem, string keyId, string channelIdentifier)
    {
        using var rsa = RSA.Create();
        rsa.ImportFromPem(privateKeyPem);

        var securityKey = new RsaSecurityKey(rsa) { KeyId = keyId };
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);

        var now = DateTime.UtcNow;
        var claims = new[]
        {
            new Claim("iss", channelIdentifier),
            new Claim("sub", channelIdentifier),
            new Claim("aud", "https://noon-api-gateway.noon.partners"),
            new Claim("jti", Guid.NewGuid().ToString())
        };

        var jwtToken = new JwtSecurityToken(
            issuer: channelIdentifier,
            audience: "https://noon-api-gateway.noon.partners",
            claims: claims,
            notBefore: now,
            expires: now.AddHours(1), // Token expiration, typical is 1 hour
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }

    /// <summary>
    /// Generates a signed JWT for Noon API authentication using KeyId for iss and sub (Session Based Auth).
    /// </summary>
    public static string GenerateSessionJwtToken(string privateKeyPem, string keyId)
    {
        using var rsa = RSA.Create();
        
        // Normalize line endings and quotes matching JS logic
        privateKeyPem = privateKeyPem.Replace("\\n", "\n").Replace("\r\n", "\n").Trim('"');
        if (privateKeyPem.Contains("-----BEGIN PRIVATE KEY-----") && !privateKeyPem.Contains("\n-----BEGIN PRIVATE KEY-----"))
        {
            privateKeyPem = privateKeyPem.Replace("-----BEGIN PRIVATE KEY-----", "-----BEGIN PRIVATE KEY-----\n")
                                         .Replace("-----END PRIVATE KEY-----", "\n-----END PRIVATE KEY-----");
        }

        rsa.ImportFromPem(privateKeyPem);

        var securityKey = new RsaSecurityKey(rsa) { KeyId = keyId };
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);

        var now = DateTime.UtcNow;
        var claims = new[]
        {
            new Claim("iss", keyId),
            new Claim("sub", keyId),
            new Claim("jti", Guid.NewGuid().ToString())
        };

        var jwtHeader = new JwtHeader(credentials);
        var jwtPayload = new JwtPayload(
            issuer: keyId,
            audience: null,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(5),
            issuedAt: now
        );

        var jwtToken = new JwtSecurityToken(jwtHeader, jwtPayload);

        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }
}
