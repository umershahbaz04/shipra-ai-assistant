using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.SharedKernel.Interfaces;

namespace Shipra.Backend.API.Application.Features.CarrierFeatures.Commands.CreateCarrierWebHook;
public class CreateCarrierWebHookCommand : IRequest<ServiceResultDTO>
{
  public int CarrierId { get; set; }
}
public class CreateCarrierWebHookCommandHandler : RequestHandlerBase<CreateCarrierWebHookCommand, ServiceResultDTO>
{
  private readonly ICarrierSharedRepository _carrierSharedRepository;
  private readonly IConfigRepository _configRepository;

  public CreateCarrierWebHookCommandHandler(ICarrierSharedRepository carrierSharedRepository, IConfigRepository configRepository, IServiceProvider serviceProvider, ILogger<CreateCarrierWebHookCommandHandler> logger) : base(serviceProvider, logger)
  {
    _carrierSharedRepository = carrierSharedRepository;
    _configRepository = configRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateCarrierWebHookCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.IntegrationKey, _currentUser.EnvironmentTypeId);
      if (mcconfig is null)
      {
        throw new EntityNotFoundException("Mcconfig", "Integration Value");
      }

      //var result = await _carrierSharedRepository.CreateWebHook(request.CarrierId, _currentUser.Id, mcconfig.Value!);

      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
