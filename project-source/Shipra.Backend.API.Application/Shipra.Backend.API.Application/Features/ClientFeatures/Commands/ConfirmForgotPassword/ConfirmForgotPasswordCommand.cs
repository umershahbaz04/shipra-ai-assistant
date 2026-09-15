using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Crmf;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ClientUseCase.Response;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Security.Service.IManagers;
using Shipra.Backend.API.SharedKernel.Interfaces;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.ConfirmForgotPassword;
public class ConfirmForgotPasswordCommand : IRequest<ServiceResultDTO>
{
  public string? UserName { get; set; }
  public string? ConfirmationCode { get; set; }
  public string? Password { get; set; }
  public string? ClientId { get; set; }
}
public class ConfirmForgotPasswordCommandHandler : RequestHandlerBase<ConfirmForgotPasswordCommand, ServiceResultDTO>
{
  private readonly IClientRepositoryInitializer _clientRepositoryInitializer;
  private readonly IKeyGeneratorManager _keyGeneratorManager;
  private readonly ISharedUserManagement _userManagement;
  private readonly IConfigRepository _configRepository;
  public ConfirmForgotPasswordCommandHandler(IClientRepositoryInitializer clientRepositoryInitializer, IKeyGeneratorManager keyGeneratorManager, ISharedUserManagement userManagement, IConfigRepository configRepository, IServiceProvider serviceProvider, ILogger<ConfirmForgotPasswordCommandHandler> logger) : base(serviceProvider, logger)
  {
    _clientRepositoryInitializer = clientRepositoryInitializer;
    _keyGeneratorManager = keyGeneratorManager;
    _userManagement = userManagement;
    _configRepository = configRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(ConfirmForgotPasswordCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.CognitoKey, _currentUser.EnvironmentTypeId);
      if (mcconfig is null)
      {
        throw new EntityNotFoundException("Mcconfig", "Cognito Value");
      }

      var result = await _userManagement.ConfirmForgotPassword(request.UserName!, request.ConfirmationCode!, request.Password!, mcconfig.Value!);
      if (!string.IsNullOrEmpty(result))
      {
        var deserialised = JsonConvert.DeserializeObject<AuthResponseModel<ConfirmUserModel>>(result);
        if (!deserialised!.isSuccess)
        {
          serviceResult.Errors = deserialised.errors;
          serviceResult.IsSuccess = false;
        }
        else
        {
          #region UpdateClientEncryptedKey

          if (!string.IsNullOrWhiteSpace(request.ClientId))
          {
            var client = await _clientRepositoryInitializer.GetClientById(request.ClientId);

            if (client != null)
            {
              var clientId = new Guid(request.ClientId);

              var keyModel = new KeyModel
              {
                ClientId = new ClientId(clientId),
                UserName = request.UserName,
                Password = request.Password,
                RandomKey = Utility.GetRandomKey(),
                PublicKey = client.PublicKey
              };

              // 🔹 Preserve old encrypted data (RandomKey & PublicKey)
              if (!string.IsNullOrEmpty(client.EncryptedKey))
              {
                var decryptedString = _keyGeneratorManager.DecryptString(client.EncryptedKey!);

                if (!string.IsNullOrEmpty(decryptedString))
                {
                  var existingKeys = JsonConvert.DeserializeObject<KeyModel>(decryptedString);

                  if (existingKeys != null)
                  {
                    keyModel.RandomKey = existingKeys.RandomKey;
                    keyModel.PublicKey = existingKeys.PublicKey;
                    keyModel.Password = request.Password;
                  }
                }
              }

              // 🔹 Encrypt updated data (with new password)
              var encryptedValue = _keyGeneratorManager.EncryptString(
                  JsonConvert.SerializeObject(keyModel)
              );

              // 🔹 Update only
              client.UpdateEncryptedKey(encryptedValue);
              await _clientRepositoryInitializer.UpdateClient(client);
            }
          }

          #endregion
          serviceResult.CreateSuccessResponse();
          serviceResult = new ServiceResultDTO(deserialised.result!);
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
public class ConfirmForgotPasswordCommandValidator : AbstractValidator<ConfirmForgotPasswordCommand>
{
  public ConfirmForgotPasswordCommandValidator()
  {
    RuleFor(v => v.UserName).NotEmpty().NotNull();
    RuleFor(v => v.ConfirmationCode).NotEmpty().NotNull();
    RuleFor(v => v.Password).NotEmpty().NotNull();
  }
}
