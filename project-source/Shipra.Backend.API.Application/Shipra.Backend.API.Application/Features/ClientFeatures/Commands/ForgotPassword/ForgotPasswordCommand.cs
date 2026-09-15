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
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.SharedKernel.Interfaces;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.ForgotPassword;
public class ForgotPasswordCommand : IRequest<ServiceResultDTO>
{ 
  public string? UserName { get; set; }  
}
public class ForgotPasswordCommandHandler : RequestHandlerBase<ForgotPasswordCommand, ServiceResultDTO>
{
  private readonly ISharedUserManagement _userManagement;
  private readonly IConfigRepository _configRepository;

  public ForgotPasswordCommandHandler(ISharedUserManagement userManagement,IConfigRepository configRepository,IServiceProvider serviceProvider, ILogger<ForgotPasswordCommandHandler> logger) : base(serviceProvider, logger)
  {
    _userManagement = userManagement;
    _configRepository = configRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(ForgotPasswordCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.CognitoKey, _currentUser.EnvironmentTypeId);
      if (mcconfig is null)
      {
        throw new EntityNotFoundException("Mcconfig", "Cognito Value");
      }

      var result = await _userManagement.ForgotPassword(request.UserName!, mcconfig.Value!);
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
  public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
  {
    public ForgotPasswordCommandValidator()
    {
      RuleFor(v => v.UserName).NotEmpty().NotNull();  
    }
  }
}
