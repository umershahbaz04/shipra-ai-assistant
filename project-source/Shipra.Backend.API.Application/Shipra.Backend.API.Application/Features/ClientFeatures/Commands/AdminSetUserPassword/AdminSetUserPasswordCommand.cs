using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ClientUseCase.Response;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.SharedKernel.Interfaces;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.AdminSetUserPassword;
public class AdminSetUserPasswordCommand : IRequest<ServiceResultDTO>
{
  public string? UserName { get; set; }
  public string? Password { get; set; }
}
public class AdminSetUserPasswordCommandHandler : RequestHandlerBase<AdminSetUserPasswordCommand, ServiceResultDTO>
{
  private readonly ISharedUserManagement _sharedUserManagement;
  private readonly IConfigRepository _configRepository;

  public AdminSetUserPasswordCommandHandler(ISharedUserManagement sharedUserManagement,IConfigRepository configRepository,IServiceProvider serviceProvider, ILogger<AdminSetUserPasswordCommandHandler> logger) : base(serviceProvider, logger)
  {
    _sharedUserManagement = sharedUserManagement;
    _configRepository = configRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(AdminSetUserPasswordCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.CognitoKey, _currentUser.EnvironmentTypeId);
      if (mcconfig is null)
      {
        throw new EntityNotFoundException("Mcconfig", "Cognito Value");
      }

      var result = await _sharedUserManagement.AdminSetUserPassword(request.UserName!, request.Password!,_currentUser.ClientIdStr, mcconfig.Value!);
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
