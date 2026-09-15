using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ClientUseCase.Response;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.SharedKernel.Interfaces;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.CheckIsUserConfirmed;

public class CheckIsUserConfirmedCommand : IRequest<ServiceResultDTO>
{
  public string? UserName { get; set; }
}
public class CheckIsUserConfirmedCommandHandler : RequestHandlerBase<CheckIsUserConfirmedCommand, ServiceResultDTO>
{
  private readonly IConfigRepository _configRepository;
  private readonly ISharedUserManagement _userManagement;

  public CheckIsUserConfirmedCommandHandler(IConfigRepository configRepository, ISharedUserManagement userManagement, IServiceProvider serviceProvider, ILogger<CheckIsUserConfirmedCommandHandler> logger) : base(serviceProvider, logger)
  {
    _configRepository = configRepository;
    _userManagement = userManagement;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CheckIsUserConfirmedCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.CognitoKey, _currentUser.EnvironmentTypeId);
      if (mcconfig is null)
      {
        throw new EntityNotFoundException("Mcconfig", "Cognito Value");
      }
      var userName = !string.IsNullOrEmpty(request.UserName) ? request.UserName : _currentUser.UserName;
      if (!string.IsNullOrEmpty(userName))
      {
        var result = await _userManagement.IsUserConfirmed(request.UserName!, mcconfig.Value!);
        if (!string.IsNullOrEmpty(result))
        {
          var deserialised = JsonConvert.DeserializeObject<AuthResponseModelWithD>(result);
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
        else
        {
          serviceResult.CreateErrorResponse(new Exception("Something went wrong!!!"));
        }
      }
      else
      {
        serviceResult.CreateErrorResponse(new Exception("Username not found in request parameter."));
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
public class ResendConfirmationCodeCommandValidator : AbstractValidator<CheckIsUserConfirmedCommand>
{
  public ResendConfirmationCodeCommandValidator()
  {
    RuleFor(x => x.UserName).NotNull().NotEmpty();
  }
}
