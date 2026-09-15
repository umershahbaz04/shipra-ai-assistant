using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ClientUseCase.Response;
using Shipra.Backend.API.Application.Features.ClientFeatures.Commands.Login;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Security.Service.IManagers;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.RotateKeys;
public class RotateKeysCommand : IRequest<ServiceResultDTO>
{
  public string? UserName { get; set; }
  public string? Password { get; set; }
}
public class RotateKeysCommandHandler : RequestHandlerBase<RotateKeysCommand, ServiceResultDTO>
{
  private readonly IMediator _mediator;
  private readonly IClientRepository _clientRepository;
  private readonly IKeyGeneratorManager _keyGeneratorManager;

  public RotateKeysCommandHandler(IMediator mediator,IClientRepository clientRepository, IKeyGeneratorManager keyGeneratorManager, IServiceProvider serviceProvider, ILogger<RotateKeysCommandHandler> logger) : base(serviceProvider, logger)
  {
    _mediator = mediator;
    _clientRepository = clientRepository;
    _keyGeneratorManager = keyGeneratorManager;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(RotateKeysCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      #region cehck username password
      var loginRequest = new LoginCommand { UserName = request.UserName, Password = request.Password };
      serviceResult = await _mediator.Send(loginRequest, cancellationToken);
      if (serviceResult.IsSuccess)
      {
        #region set data
        string? encryptedValue = string.Empty;
        var keyModel = new KeyModel()
        {
          ClientId = _currentUser.ClientId,
          PublicKey = Utility.GetPublicKey(),
          RandomKey = Utility.GetRandomKey(),
          UserName = request.UserName,
          Password = request.Password,
        };
        var content = JsonConvert.SerializeObject(keyModel);
        encryptedValue = _keyGeneratorManager.EncryptString(content);

        var client = await _clientRepository.GetClientById(_currentUser.ClientId!);
        if (client is null)
        {
          throw new EntityNotFoundException("Client", _currentUser.ClientId!);
        }

        client.RotateKeys(keyModel.PublicKey, Utility.GetSecretKey(), encryptedValue);
        await _clientRepository.UpdateClient(client);
        serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = _currentUser.ClientIdStr, Message = "Key rotate successfully" });
        #endregion
      }
      #endregion
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
