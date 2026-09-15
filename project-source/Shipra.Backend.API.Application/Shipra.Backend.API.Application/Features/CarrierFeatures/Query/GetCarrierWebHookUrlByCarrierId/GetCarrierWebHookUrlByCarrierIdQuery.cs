using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.SharedKernel.Interfaces;

namespace Shipra.Backend.API.Application.Features.CarrierFeatures.Query.GetCarrierWebHookUrlByCarrierId;
public class GetCarrierWebHookUrlByCarrierIdQuery : IRequest<ServiceResultDTO>
{
  public int CarrierId { get; set; }
  public int? ActiveCarrierId { get; set; }
  public int? CarrierContractTypeId { get; set; }
}
public class GetCarrierWebHookUrlByCarrierIdQueryHandler : RequestHandlerBase<GetCarrierWebHookUrlByCarrierIdQuery, ServiceResultDTO>
{
  private readonly ICarrierSharedRepository _carrierSharedRepository;
  private readonly IConfigRepository _configRepository;
  private readonly ICarrierRepository _carrierRepository;

  public GetCarrierWebHookUrlByCarrierIdQueryHandler(ICarrierSharedRepository carrierSharedRepository, IConfigRepository configRepository, ICarrierRepository carrierRepository, IServiceProvider serviceProvider, ILogger<GetCarrierWebHookUrlByCarrierIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _carrierSharedRepository = carrierSharedRepository;
    _configRepository = configRepository;
    _carrierRepository = carrierRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetCarrierWebHookUrlByCarrierIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {

      var carrier = await _carrierRepository.GetCarrierFromMasterDbById(request.CarrierId);
      if (carrier == null)
      {
        throw new EntityNotFoundException("Carrier", request.CarrierId);
      }
      Guid? clientId = null; 
      WebhookUrlHash? exitedWebhookHash = null;
      if (request.CarrierContractTypeId == (int)EnumCarrierContractType.ShipraContractType)
      {
        exitedWebhookHash = await _carrierRepository.CheckWebhookUrlHashByContractIdAndCarrierId(request.CarrierId, request.CarrierContractTypeId.GetValueOrDefault()); 
      }
      else
      {
        clientId = new Guid(_currentUser.ClientIdStr!);
        exitedWebhookHash = await _carrierRepository.CheckWebhookUrlHashByClientAndCarrierId(request.CarrierId, new Guid(_currentUser.ClientIdStr!)!);
      }
      if (exitedWebhookHash is null)
      {
        exitedWebhookHash = WebhookUrlHash.CreateWebhookUrlHash(request.CarrierId, request.ActiveCarrierId, clientId!, _currentUser.EmployeeId, request.CarrierContractTypeId);
        bool created = await _carrierRepository.CreateWebhookUrlHash(exitedWebhookHash);
      }

      #region if dispatchex carrier

      var oIntegrationWebHook = await _configRepository.GetMcconfigByKey(ApplicationConstants.IntegrationWebHookKey, _currentUser.EnvironmentTypeId);
      if (oIntegrationWebHook is null)
      {
        throw new EntityNotFoundException("Mcconfig", "Integration WebHook Value");
      }
      #region create webhook entry for status update on shipra from third part
      /////// create webhook url on thirdparty
      var oIntegration = await _configRepository.GetMcconfigByKey(ApplicationConstants.IntegrationKey, _currentUser.EnvironmentTypeId);
      string integrationBaseUrl = oIntegrationWebHook!.Value!;
      string integrationRequestUri = $"api/Tracking/WebHook?sig={exitedWebhookHash.WebhookUrlHashId}";
      string webhookUrl = $"{integrationBaseUrl}/{integrationRequestUri}";
      if (carrier.IsDispatchExCompany.GetValueOrDefault())
      {
        try
        {
          var data = await _carrierSharedRepository.CreateWebHook(request.CarrierId, request.ActiveCarrierId.GetValueOrDefault(), _currentUser.ClientIdStr!, webhookUrl, oIntegration!.Value!,request.CarrierContractTypeId.GetValueOrDefault());
        }
        catch (Exception)
        {
          Console.WriteLine("Exception caught!");
        }
      }
      serviceResult = new ServiceResultDTO(new { webhookUrl = webhookUrl });

      #endregion

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
public class GetCarrierWebHookUrlByCarrierIdQueryValidator : AbstractValidator<GetCarrierWebHookUrlByCarrierIdQuery>
{
  public GetCarrierWebHookUrlByCarrierIdQueryValidator()
  {
    When(v => v.ActiveCarrierId! == 0, () =>
    {
      RuleFor(x => x.CarrierContractTypeId).NotNull().NotEmpty().GreaterThan(0); 
    });  
    When(v => v.CarrierContractTypeId! == 0, () =>
    {
      RuleFor(x => x.ActiveCarrierId).NotNull().NotEmpty().GreaterThan(0); 
    });
    RuleFor(x => x.CarrierId).NotNull().NotEmpty().GreaterThan(0);
  }
}
