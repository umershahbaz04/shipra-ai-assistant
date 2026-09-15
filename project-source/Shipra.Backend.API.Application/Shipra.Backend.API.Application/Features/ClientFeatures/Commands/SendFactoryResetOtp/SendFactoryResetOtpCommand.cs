using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.SendFactoryResetOtp;

/// <summary>
/// Command to send a factory reset OTP to the current user's registered email.
/// No request body is required — the client is resolved from the authenticated user context.
/// </summary>
public class SendFactoryResetOtpCommand : IRequest<ServiceResultDTO>
{
}
