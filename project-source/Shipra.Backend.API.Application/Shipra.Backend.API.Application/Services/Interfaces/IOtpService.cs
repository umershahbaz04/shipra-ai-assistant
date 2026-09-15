namespace Shipra.Backend.API.Application.Services.Interfaces;

/// <summary>
/// Provides OTP (One-Time Password) generation functionality.
/// </summary>
public interface IOtpService
{
  /// <summary>
  /// Generates a cryptographically secure 6-digit OTP.
  /// </summary>
  /// <returns>A 6-digit numeric OTP string.</returns>
  string GenerateOtp();
}
