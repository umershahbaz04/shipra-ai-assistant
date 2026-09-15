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
using Shipra.Backend.API.Application.Features.ClientFeatures.Commands.ResendConfirmationCode;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.SharedKernel.Interfaces;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.GetAccessTokenWithRefreshToken;
public class GetAccessTokenWithRefreshTokenCommand : IRequest<ServiceResultDTO>
{
  public string? UserName { get; set; }
  public string? RefreshToken { get; set; }
}
public class GetAccessTokenWithRefreshTokenCommandHandler : RequestHandlerBase<GetAccessTokenWithRefreshTokenCommand, ServiceResultDTO>
{
  private readonly IConfigRepository _configRepository;
  private readonly ISharedUserManagement _userManagement;

  public GetAccessTokenWithRefreshTokenCommandHandler(IConfigRepository configRepository, ISharedUserManagement userManagement,IServiceProvider serviceProvider, ILogger<GetAccessTokenWithRefreshTokenCommandHandler> logger) : base(serviceProvider, logger)
  {
    _configRepository = configRepository;
    _userManagement = userManagement;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAccessTokenWithRefreshTokenCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.CognitoKey, _currentUser.EnvironmentTypeId);
      if (mcconfig is null)
      {
        throw new EntityNotFoundException("Mcconfig", "Cognito Value");
      }


      var result = await _userManagement.GetAccessTokenWithRefreshTokenAsync(request.UserName!, request.RefreshToken!, mcconfig.Value!);
      if (!string.IsNullOrEmpty(result))
      {
        var deserialised = JsonConvert.DeserializeObject<AuthResponseModel<LoginResult>>(result);
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
public class GetAccessTokenWithRefreshTokenCommandValidator : AbstractValidator<GetAccessTokenWithRefreshTokenCommand>
{
  public GetAccessTokenWithRefreshTokenCommandValidator()
  {
    RuleFor(x => x.UserName).NotNull().NotEmpty();
    RuleFor(x => x.RefreshToken).NotNull().NotEmpty();
  }
}
