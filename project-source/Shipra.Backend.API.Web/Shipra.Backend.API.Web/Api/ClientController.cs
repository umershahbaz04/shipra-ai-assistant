using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Common.Security;
using Shipra.Backend.API.Application.Features.ClientFeatures.Commands.CreateUpdateClientGenericSetting;
using Shipra.Backend.API.Application.Features.ClientFeatures.Commands.DeleteClient;
using Shipra.Backend.API.Application.Features.ClientFeatures.Commands.WipeOutClientData;
using Shipra.Backend.API.Application.Features.ClientFeatures.Commands.MarkClientPaymentVerification;
using Shipra.Backend.API.Application.Features.ClientFeatures.Commands.RotateKeys;
using Shipra.Backend.API.Application.Features.ClientFeatures.Commands.StripeCancelSubscription;
using Shipra.Backend.API.Application.Features.ClientFeatures.Commands.StripeClientPaymentMethodDetach;
using Shipra.Backend.API.Application.Features.ClientFeatures.Commands.StripeCreateSubscription;
using Shipra.Backend.API.Application.Features.ClientFeatures.Commands.StripeUpdateDefaultPaymentMethod;
using Shipra.Backend.API.Application.Features.ClientFeatures.Commands.StripeUpdateSubscriptionRemoveTrial;
using Shipra.Backend.API.Application.Features.ClientFeatures.Commands.UpdateClient;
using Shipra.Backend.API.Application.Features.ClientFeatures.Commands.UpdateClientProfileImage;
using Shipra.Backend.API.Application.Features.ClientFeatures.Commands.CreateClientCarrierTrackingStatus;
using Shipra.Backend.API.Application.Features.ClientFeatures.Query.GetActiveClients;
using Shipra.Backend.API.Application.Features.ClientFeatures.Query.GetAllClients;
using Shipra.Backend.API.Application.Features.ClientFeatures.Query.GetClientById;
using Shipra.Backend.API.Application.Features.ClientFeatures.Query.GetClientKeys;
using Shipra.Backend.API.Application.Features.ClientFeatures.Query.GetClientProfile;
using Shipra.Backend.API.Application.Features.ClientFeatures.Query.GetGenericSetting;
using Shipra.Backend.API.Application.Features.ClientFeatures.Query.GetWipeOutSectionsLookup;
using Shipra.Backend.API.Application.Features.ClientFeatures.Query.GetRegionTimeZoneByClient;
using Shipra.Backend.API.Application.Features.ClientFeatures.Query.GetStripeClientSecret;
using Shipra.Backend.API.Application.Features.ClientFeatures.Query.GetValidateClientPPActivate;
using Shipra.Backend.API.Application.Features.ClientFeatures.Query.StripeGetAllProducts;
using Shipra.Backend.API.Application.Features.ClientFeatures.Query.StripeGetClientPaymentMethods;
using Shipra.Backend.API.Application.Features.ClientFeatures.Query.StripeGetClientSubscription;
using Shipra.Backend.API.Application.Features.ClientFeatures.Query.StripeGetDefaultPaymentMethod;
using Shipra.Backend.API.Application.Features.ClientFeatures.Commands.SendFactoryResetOtp;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GenerateManifest;

namespace Shipra.Backend.API.Web.Api;
[Authorize]
public class ClientController : BaseApiController
{
  public ClientController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }
  #region command
  [HttpPost("UpdateClient")]
  public async Task<ActionResult> UpdateClient([FromBody] UpdateClientCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("DeleteClientById")]
  public async Task<ActionResult> DeleteClientById([FromBody] DeleteClientCommand request, CancellationToken cancellationToken = default)
  { 
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("WipeOutClientData")]
  public async Task<ActionResult> WipeOutClientData([FromBody] WipeOutClientDataCommand request, CancellationToken cancellationToken = default)
  { 
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("UpdateClientProfileImage")]
  public async Task<ActionResult> UpdateClientProfileImage([FromForm] UpdateClientProfileImageCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("MarkClientPaymentVerification")]
  public async Task<ActionResult> MarkClientPaymentVerification([FromBody] MarkClientPaymentVerificationCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("CancelClientSubscription")]
  public async Task<ActionResult> CancelClientSubscription([FromBody] StripeCancelSubscriptionCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("ClientPaymentMethodDetach")]
  public async Task<ActionResult> ClientPaymentMethodDetach([FromBody] StripeClientPaymentMethodDetachCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("UpdateClientDefaultPaymentMethod")]
  public async Task<ActionResult> UpdateClientDefaultPaymentMethod([FromBody] StripeUpdateDefaultPaymentMethodCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("CreateSubscription")]
  public async Task<ActionResult> CreateSubscription([FromBody] StripeCreateSubscriptionCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("UpdateSubscriptionRemoveTrial")]
  public async Task<ActionResult> UpdateSubscriptionRemoveTrial([FromBody] StripeUpdateSubscriptionRemoveTrialCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("CreateUpdateClientGenericSetting")]
  public async Task<ActionResult> CreateUpdateClientGenericSetting([FromBody] CreateUpdateClientGenericSettingCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("SendFactoryResetOtp")]
  public async Task<ActionResult> SendFactoryResetOtp(CancellationToken cancellationToken = default)
  {
    SendFactoryResetOtpCommand request = new SendFactoryResetOtpCommand();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("CreateClientCarrierTrackingStatus")]
  public async Task<ActionResult> CreateClientCarrierTrackingStatus([FromBody] CreateClientCarrierTrackingStatusCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  #endregion

  #region query
  [HttpGet("GetClientById")]
  public async Task<ActionResult> GetClientById([FromQuery] GetClientByIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetAllClients")]
  public async Task<ActionResult> GetAllClients([FromBody] GetAllClientsQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("RotateKeys")]
  public async Task<ActionResult> RotateKeys([FromBody] RotateKeysCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetActiveClients")]
  public async Task<ActionResult> GetActiveClients([FromBody] GetActiveClientsQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetClientProfile")]
  public async Task<ActionResult> GetClientProfile(CancellationToken cancellationToken = default)
  {
    GetClientProfileQuery request = new GetClientProfileQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetStripeClientSecret")]
  public async Task<ActionResult> GetStripeClientSecret(CancellationToken cancellationToken = default)
  {
    GetStripeClientSecretQuery request = new GetStripeClientSecretQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetValidateClientPPActivate")]
  public async Task<ActionResult> GetValidateClientPPActivate(CancellationToken cancellationToken = default)
  {
    GetValidateClientPPActivateQuery request = new GetValidateClientPPActivateQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetRegionTimeZone")]
  public async Task<ActionResult> GetRegionTimeZone(CancellationToken cancellationToken = default)
  {
    GetRegionTimeZoneByClientQuery request = new GetRegionTimeZoneByClientQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetClientPaymentMethods")]
  public async Task<ActionResult> GetClientPaymentMethods(CancellationToken cancellationToken = default)
  {
    StripeGetClientPaymentMethodsQuery request = new StripeGetClientPaymentMethodsQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetClientSubscription")]
  public async Task<ActionResult> GetClientSubscription(CancellationToken cancellationToken = default)
  {
    StripeGetClientSubscriptionQuery request = new StripeGetClientSubscriptionQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("StripeGetAllProducts")]
  public async Task<ActionResult> StripeGetAllProducts(CancellationToken cancellationToken = default)
  {
    StripeGetAllProductsQuery request = new StripeGetAllProductsQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetDefaultPaymentMethod")]
  public async Task<ActionResult> GetDefaultPaymentMethod(CancellationToken cancellationToken = default)
  {
    StripeGetDefaultPaymentMethodQuery request = new StripeGetDefaultPaymentMethodQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetClientKeys")]
  public async Task<ActionResult> GetClientKeys(CancellationToken cancellationToken = default)
  {
    GetClientKeysQuery request = new GetClientKeysQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetGenericSetting")]
  public async Task<ActionResult> GetGenericSetting(CancellationToken cancellationToken = default)
  {
    GetGenericSettingQuery request = new GetGenericSettingQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetWipeOutSectionsLookup")]
  public async Task<ActionResult> GetWipeOutSectionsLookup(CancellationToken cancellationToken = default)
  {
    GetWipeOutSectionsLookupQuery request = new GetWipeOutSectionsLookupQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion
}
