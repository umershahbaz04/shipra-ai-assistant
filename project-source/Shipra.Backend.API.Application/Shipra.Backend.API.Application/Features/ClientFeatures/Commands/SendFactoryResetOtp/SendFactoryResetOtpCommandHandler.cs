using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.SendFactoryResetOtp;

/// <summary>
/// Handles the factory reset OTP flow:
/// 1. Fetches the client's email from the Client table using the current user's ID.
/// 2. Generates a 6-digit OTP via IOtpService.
/// 3. Sends a professionally styled email containing the OTP.
/// </summary>
public class SendFactoryResetOtpCommandHandler : RequestHandlerBase<SendFactoryResetOtpCommand, ServiceResultDTO>
{
  private readonly IClientRepository _clientRepository;
  private readonly IOtpService _otpService;

  public SendFactoryResetOtpCommandHandler(
    IClientRepository clientRepository,
    IOtpService otpService,
    IServiceProvider serviceProvider,
    ILogger<SendFactoryResetOtpCommandHandler> logger) : base(serviceProvider, logger)
  {
    _clientRepository = clientRepository;
    _otpService = otpService;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(SendFactoryResetOtpCommand request, CancellationToken cancellationToken)
  {
    var response = new ServiceResultDTO();
    try
    {
      #region Fetch client and validate email
      var client = await _clientRepository.GetClientById(_currentUser.ClientId!);

      if (_currentUser.RoleId != (int)EnumUserRole.Admin)
      {
        throw new Exception("Unauthorized or client session not found.");
      }
      if (client is null)
      {
        throw new EntityNotFoundException("Client", _currentUser.ClientId!);
      }

      if (string.IsNullOrEmpty(client.Email))
      {
        throw new EntityNotFoundException("Client Email", _currentUser.ClientId!);
      }
      #endregion

      #region Generate OTP
      var otp = _otpService.GenerateOtp();
      #endregion

      #region Build email body and send
      string body = $@"
        <div style='font-family: Lato, Arial, sans-serif; color: #333; max-width: 600px; margin: 0 auto;'>
            <h2 style='color: #563AD5;'>Factory Reset OTP</h2>
            <p>Dear User,</p>
            <p>This is your OTP for factory reset:</p>
            <div style='text-align: center; margin: 30px 0;'>
                <span style='font-size: 32px; font-weight: bold; letter-spacing: 8px; color: #563AD5; 
                             background-color: #F4F1FE; padding: 15px 30px; border-radius: 8px; 
                             border: 2px dashed #563AD5; display: inline-block;'>{otp}</span>
            </div>
            <p>Please use this OTP to complete your factory reset process. This OTP is valid for a limited time.</p>
            <p style='font-size: 12px; color: #888;'>Note: If you did not request a factory reset, please ignore this email or contact our support team immediately at <a href='mailto:{ApplicationConstants.ShipraSupportEmail}' style='color: #563AD5;'>{ApplicationConstants.ShipraSupportEmail}</a>.</p>
            <br />
            <p>Best regards,</p>
            <p><strong>Shipra Team</strong></p>
            <div style='text-align: left;'>
                <img src='{ApplicationConstants.ShipraLogo}' alt='Shipra Logo' style='width: 150px; height: auto; margin-bottom: 20px;' />
            </div>
        </div>";

      await _emailServiceProvider.SendEmailAsync("Factory Reset OTP - Shipra", body, new string[] { client.Email! });
      #endregion

      response = new ServiceResultDTO(new BaseResponseDto
      {
        Data = otp,
        Message = "OTP has been sent to your registered email address."
      });

      return response;
    }
    catch (Exception ex)
    {
      response.CreateErrorResponse(ex);
      throw;
    }
  }
}
