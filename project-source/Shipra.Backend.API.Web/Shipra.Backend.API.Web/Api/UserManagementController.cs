using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Features.ClientFeatures.Commands.AdminSetUserPassword;
using Shipra.Backend.API.Application.Features.ClientFeatures.Commands.CheckEmailAvailability;
using Shipra.Backend.API.Application.Features.ClientFeatures.Commands.CheckIsUserConfirmed;
using Shipra.Backend.API.Application.Features.ClientFeatures.Commands.CheckUsernameAvailability;
using Shipra.Backend.API.Application.Features.ClientFeatures.Commands.ConfirmForgotPassword;
using Shipra.Backend.API.Application.Features.ClientFeatures.Commands.ConfirmSignUpUser;
using Shipra.Backend.API.Application.Features.ClientFeatures.Commands.CreateClient;
using Shipra.Backend.API.Application.Features.ClientFeatures.Commands.ForgotPassword;
using Shipra.Backend.API.Application.Features.ClientFeatures.Commands.GetAccessTokenWithRefreshToken;
using Shipra.Backend.API.Application.Features.ClientFeatures.Commands.Login;
using Shipra.Backend.API.Application.Features.ClientFeatures.Commands.Logout;
using Shipra.Backend.API.Application.Features.ClientFeatures.Commands.ResendConfirmationCode;
using Shipra.Backend.API.Application.Features.ClientFeatures.Commands.UpdatePassword;
using Shipra.Backend.API.Application.Features.ClientFeatures.Commands.UploadClientImage;
using Shipra.Backend.API.Application.Features.ClientFeatures.Query.GetClientByKey;

namespace Shipra.Backend.API.Web.Api;

public class UserManagementController : BaseApiController
{
  public UserManagementController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }
  #region command
  [HttpPost("Signup")]
  public async Task<ActionResult> Signup([FromBody] CreateClientCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("CheckUsernameAvailability")]
  public async Task<ActionResult> CheckUsernameAvailability([FromBody] CheckUsernameAvailabilityCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("CheckEmailAvailability")]
  public async Task<ActionResult> CheckEmailAvailability([FromBody] CheckEmailAvailabilityCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("CheckIsUserConfirmed")]
  public async Task<ActionResult> CheckIsUserConfirmed([FromBody] CheckIsUserConfirmedCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion

  #region cognito command
  [HttpPost("UpdatePassword")]
  public async Task<ActionResult> UpdatePassword([FromBody] UpdatePasswordCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("ConfirmSignUpUser")]
  public async Task<ActionResult> ConfirmSignUpUser([FromBody] ConfirmSignUpUserCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("ResendConfirmationCode")]
  public async Task<ActionResult> ResendConfirmationCode([FromBody] ResendConfirmationCodeCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("Login")]
  public async Task<ActionResult> Login([FromBody] LoginCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("Logout")]
  public async Task<ActionResult> Logout([FromBody] LogoutCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetAccessTokenWithRefreshToken")]
  public async Task<ActionResult> GetAccessTokenWithRefreshToken([FromBody] GetAccessTokenWithRefreshTokenCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("UploadClientImage")]
  public async Task<ActionResult> UploadClientImage([FromForm] UploadClientImageCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("ForgotPassword")]
  public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("ConfirmForgotPassword")]
  public async Task<ActionResult> ConfirmForgotPassword([FromBody] ConfirmForgotPasswordCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("AdminSetUserPassword")]
  public async Task<ActionResult> AdminSetUserPassword([FromBody] AdminSetUserPasswordCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("GetClientByKey")]
  public async Task<ActionResult> GetClientByKey([FromBody] GetClientByKeyQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion
}
