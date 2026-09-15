using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ClientUseCase.Response;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Security.Service.IManagers;
using Shipra.Backend.API.Security.Service.Managers;
using Shipra.Backend.API.SharedKernel.Interfaces;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.UpdatePassword;
public class UpdatePasswordCommand : IRequest<ServiceResultDTO>
{
  public string? OldPassword { get; set; }
  public string? NewPassword { get; set; }
}
public class UpdatePasswordCommandHandler : RequestHandlerBase<UpdatePasswordCommand, ServiceResultDTO>
{
  private readonly IConfigRepository _configRepository;
  private readonly ISharedUserManagement _sharedUserManagement;

  public UpdatePasswordCommandHandler(IClientRepositoryInitializer clientRepositoryInitializer, IKeyGeneratorManager keyGeneratorManager, IConfigRepository configRepository, ISharedUserManagement sharedUserManagement, IServiceProvider serviceProvider, ILogger<UpdatePasswordCommandHandler> logger) : base(serviceProvider, logger)
  {
    ClientRepositoryInitializer = clientRepositoryInitializer;
    KeyGeneratorManager = keyGeneratorManager;
    _configRepository = configRepository;
    _sharedUserManagement = sharedUserManagement;
  }

  public IClientRepositoryInitializer ClientRepositoryInitializer { get; }
  public IKeyGeneratorManager KeyGeneratorManager { get; }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdatePasswordCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.CognitoKey, _currentUser.EnvironmentTypeId);
      if (mcconfig is null)
      {
        throw new EntityNotFoundException("Mcconfig", "Cognito Value");
      }

      var result = await _sharedUserManagement.UpdatePassword(request.OldPassword!, request.NewPassword, _currentUser.AccessToken, mcconfig.Value!);
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

            var client = await ClientRepositoryInitializer.GetClientById(_currentUser.ClientIdStr!);

            if (client != null)
            {

              var keyModel = new KeyModel
              {
                ClientId = _currentUser.ClientId!,
                UserName = client.Username,
                Password = request.NewPassword,
                RandomKey = Utility.GetRandomKey(),
                PublicKey = client.PublicKey
              };

              // 🔹 Preserve old encrypted data (RandomKey & PublicKey)
              if (!string.IsNullOrEmpty(client.EncryptedKey))
              {
                var decryptedString = KeyGeneratorManager.DecryptString(client.EncryptedKey!);

                if (!string.IsNullOrEmpty(decryptedString))
                {
                  var existingKeys = JsonConvert.DeserializeObject<KeyModel>(decryptedString);

                  if (existingKeys != null)
                  {
                  keyModel.RandomKey = existingKeys.RandomKey;
                  keyModel.PublicKey = existingKeys.PublicKey;
                  keyModel.Password = request.NewPassword;
                }
                }
              }

              // 🔹 Encrypt updated data (with new password)
              var encryptedValue = KeyGeneratorManager.EncryptString(
                  JsonConvert.SerializeObject(keyModel)
              );

              // 🔹 Update only
              client.UpdateEncryptedKey(encryptedValue);
              await ClientRepositoryInitializer.UpdateClient(client);
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
public class UpdatePasswordCommandValidator : AbstractValidator<UpdatePasswordCommand>
{
  public UpdatePasswordCommandValidator()
  {
    RuleFor(x => x.OldPassword).NotEmpty().NotNull();
    RuleFor(x => x.NewPassword).NotEmpty().NotNull();
  }
}
