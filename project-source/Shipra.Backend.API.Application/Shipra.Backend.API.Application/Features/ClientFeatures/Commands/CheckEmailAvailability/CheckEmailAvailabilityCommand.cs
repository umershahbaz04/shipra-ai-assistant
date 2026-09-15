using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.SharedKernel.Interfaces;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.CheckEmailAvailability;
public class CheckEmailAvailabilityCommand : IRequest<ServiceResultDTO>
{
  public string? Email { get; set; }
}
public class CheckEmailAvailabilityCommandHandler : RequestHandlerBase<CheckEmailAvailabilityCommand, ServiceResultDTO>
{
  private readonly IConfigRepository _configRepository;
  private readonly ISharedUserManagement _userManagement;

  public CheckEmailAvailabilityCommandHandler(IConfigRepository configRepository, ISharedUserManagement userManagement, IServiceProvider serviceProvider, ILogger<CheckEmailAvailabilityCommandHandler> logger) : base(serviceProvider, logger)
  {
    _configRepository = configRepository;
    _userManagement = userManagement;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CheckEmailAvailabilityCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.Admin, _currentUser.EnvironmentTypeId);
      if (mcconfig is null)
      {
        throw new EntityNotFoundException("Mcconfig", "Cognito Value");
      }
      var isEmailAvailable = await _userManagement.CheckEmailAvailability(request.Email!, mcconfig.Value!);

      if (isEmailAvailable is not null && Convert.ToBoolean(isEmailAvailable))
      {
        serviceResult.Errors!["EmailExists"] = new string[] { "Email already exists: " + request.Email };
        serviceResult.CreateErrorResponse(HttpStatusCode.BadRequest);
      }
      else
      {
        serviceResult.CreateSuccessResponse(HttpStatusCode.OK);
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
