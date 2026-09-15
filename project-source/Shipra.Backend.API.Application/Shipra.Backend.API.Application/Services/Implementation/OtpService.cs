using System.Security.Cryptography;
using Shipra.Backend.API.Application.Services.Interfaces;

namespace Shipra.Backend.API.Application.Services.Implementation;

/// <summary>
/// Generates cryptographically secure one-time passwords.
/// </summary>
public class OtpService : IOtpService
{
  /// <summary>
  /// Generates a cryptographically secure 6-digit OTP.
  /// </summary>
  /// <returns>A 6-digit numeric OTP string (e.g., "042917").</returns>
  public string GenerateOtp()
  {
    var otp = RandomNumberGenerator.GetInt32(0, 1000000);
    return otp.ToString("D6");
  }
}
