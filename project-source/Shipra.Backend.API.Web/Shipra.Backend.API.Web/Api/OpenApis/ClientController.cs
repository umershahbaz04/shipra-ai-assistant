using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Features.ClientFeatures.Commands.AddUpdateClientConfigSetting;
using Shipra.Backend.API.Application.Features.ClientFeatures.Commands.LoginWithEncryptedKey;
using Shipra.Backend.API.Application.Features.ClientFeatures.Commands.UpdateAllowPersonalClientCarrierContract;
using Shipra.Backend.API.Application.Features.ClientFeatures.Commands.UpdateAllowWithoutBalance;
using Shipra.Backend.API.Application.Features.ClientFeatures.Query.GetSecretKeyDecryptedUserNamePassword;

namespace Shipra.Backend.API.Web.Api.OpenApis;

public class ClientController : BaseApiController
{
  public ClientController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }

  #region command
  [HttpPost("UpdateAllowWithoutBalance")]
  public async Task<ActionResult> UpdateAllowWithoutBalance([FromBody] UpdateAllowWithoutBalanceCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("UpdateAllowPersonalClientCarrierContract")]
  public async Task<ActionResult> UpdateAllowPersonalClientCarrierContract([FromBody] UpdateAllowPersonalClientCarrierContractCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("UpdateClientConfigSettingWithAutoRefreshTime")]
  public async Task<ActionResult> UpdateClientConfigSettingWithAutoRefreshTime([FromBody] UpdateClientConfigSettingWithAutoRefreshTimeCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("LoginWithEncryptedKey")]
  public async Task<ActionResult> LoginWithEncryptedKey([FromBody] LoginWithEncryptedKeyCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("DecryptEncryptedKey")]
  public async Task<ActionResult> DecryptEncryptedKey([FromBody] GetSecretKeyDecryptedUserNamePasswordQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion
}
