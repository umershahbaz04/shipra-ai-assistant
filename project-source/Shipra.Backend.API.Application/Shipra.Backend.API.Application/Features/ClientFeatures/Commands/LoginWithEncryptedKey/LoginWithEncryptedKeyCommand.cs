using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Crmf;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ClientUseCase.Response;
using Shipra.Backend.API.Application.Features.ClientFeatures.Commands.Login;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Security.Service.IManagers;
using Shipra.Backend.API.SharedKernel.Models;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.LoginWithEncryptedKey;
public class LoginWithEncryptedKeyCommand : IRequest<ServiceResultDTO>
{
  public string? Key { get; set; }
}
public class LoginWithEncryptedKeyCommandHandler : RequestHandlerBase<LoginWithEncryptedKeyCommand, ServiceResultDTO>
{
  private readonly IClientLoginRepository _clientRepository;
  private readonly ICurrentTenantService _currentTenantService;
  private readonly IMediator _mediator;
  private readonly IKeyGeneratorManager _keyGeneratorManager;

  public LoginWithEncryptedKeyCommandHandler(IClientLoginRepository clientRepository, ICurrentTenantService currentTenantService, IMediator mediator, IKeyGeneratorManager keyGeneratorManager, IServiceProvider serviceProvider, ILogger<LoginWithEncryptedKeyCommandHandler> logger) : base(serviceProvider, logger)
  {
    _clientRepository = clientRepository;
    _currentTenantService = currentTenantService;
    _mediator = mediator;
    _keyGeneratorManager = keyGeneratorManager;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(LoginWithEncryptedKeyCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var decryptedKey = string.Empty;
      try
      {
        decryptedKey = _keyGeneratorManager.DecryptString(request.Key!);

      }
      catch (Exception)
      {
        serviceResult.CreateError("InvalidKey", new string[] { "The provided secret key is invalid. Please check and try again." });
        return serviceResult;
      }
      if (!string.IsNullOrEmpty(decryptedKey))
      {
        var keyModel = JsonConvert.DeserializeObject<KeyModel>(decryptedKey!);
        if (keyModel is not null)
        {
          if (keyModel.ClientId != null)
          {
            var connectionString = _currentTenantService.GetConnectionStringByTenant(keyModel!.ClientId!.Value!.ToString());

            var client = await _clientRepository.GetClientByClientId(keyModel!.ClientId!.Value!.ToString(), connectionString);
             
            if (client is null)
            {
              throw new EntityNotFoundException("Client", keyModel!.ClientId!.Value!.ToString());
            }

            if (!string.Equals(client?.EncryptedKey, request.Key, StringComparison.OrdinalIgnoreCase))
            {
              throw new Exception("The provided strings do not match. Please retrieve a new one from the profile security section.");
            }


            // Call another MediatR request from within this handler
            var loginRequest = new LoginCommand { UserName = keyModel.UserName, Password = keyModel.Password };
            serviceResult = await _mediator.Send(loginRequest, cancellationToken);
          }
          else
          {
            throw new Exception("User not found. please rotate your key.");
          }
        }
      }
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
