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

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.CheckUsernameAvailability;
public class CheckUsernameAvailabilityCommand : IRequest<ServiceResultDTO>
{
  public string? UserName { get; set; }
}
public class CheckUsernameAvailabilityCommandHandler : RequestHandlerBase<CheckUsernameAvailabilityCommand, ServiceResultDTO>
{
  private readonly IConfigRepository _configRepository;
  private readonly ISharedUserManagement _userManagement;

  public CheckUsernameAvailabilityCommandHandler(IConfigRepository configRepository, ISharedUserManagement userManagement, IServiceProvider serviceProvider, ILogger<CheckUsernameAvailabilityCommandHandler> logger) : base(serviceProvider, logger)
  {
    _configRepository = configRepository;
    _userManagement = userManagement;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CheckUsernameAvailabilityCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.CognitoKey, _currentUser.EnvironmentTypeId);
      if (mcconfig is null)
      {
        throw new EntityNotFoundException("Mcconfig", "Cognito Value");
      }
      string result = await _userManagement.CheckUsernameAvailability(request.UserName!, mcconfig.Value!);
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
public class CheckUsernameAvailabilityCommandValidator : AbstractValidator<CheckUsernameAvailabilityCommand>
{
  public CheckUsernameAvailabilityCommandValidator()
  {
    RuleFor(x => x.UserName).NotNull().NotEmpty();
  }
}
