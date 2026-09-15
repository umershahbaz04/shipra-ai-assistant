using FluentValidation;
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

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.ConfirmSignUpUser;
public class ConfirmSignUpUserCommand : IRequest<ServiceResultDTO>
{
  public string? UserName { get; set; }
  public string? Code { get; set; }
}
public class ConfirmSignUpUserCommandHandler : RequestHandlerBase<ConfirmSignUpUserCommand, ServiceResultDTO>
{
  private readonly IConfigRepository _configRepository;
  private readonly ISharedUserManagement _userManagement;

  public ConfirmSignUpUserCommandHandler(IConfigRepository configRepository, ISharedUserManagement userManagement, IServiceProvider serviceProvider, ILogger<ConfirmSignUpUserCommandHandler> logger) : base(serviceProvider, logger)
  {
    _configRepository = configRepository;
    _userManagement = userManagement;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(ConfirmSignUpUserCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.CognitoKey, _currentUser.EnvironmentTypeId);
      if (mcconfig is null)
      {
        throw new EntityNotFoundException("Mcconfig", "Cognito Value");
      }


      var result = await _userManagement.ConfirmSignUpRequestAsync(request.UserName!, request.Code!, mcconfig.Value!);
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
public class ConfirmSignUpUserCommandValidator : AbstractValidator<ConfirmSignUpUserCommand>
{
  public ConfirmSignUpUserCommandValidator()
  {
    RuleFor(x => x.UserName).NotNull().NotEmpty();
    RuleFor(x => x.Code).NotNull().NotEmpty();
  }
}
