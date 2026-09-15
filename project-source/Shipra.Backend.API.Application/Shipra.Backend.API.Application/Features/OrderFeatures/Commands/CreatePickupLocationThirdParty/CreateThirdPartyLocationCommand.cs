using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.CarrierUseCase;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.RefreshCarrierStatus;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.SharedKernel.Interfaces;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.UploadPaperlessDocuments;
public class CreateThirdPartyLocationCommand : IRequest<ServiceResultDTO>
{
  public decimal? Weight { get; set; }
  public int? Quantity { get; set; }
  public string? ServiceCode { get; set; }
  public string? ContainerCode { get; set; }
  public int ActiveCarrierId { get; set; }
  public int carrierid { get; set; }
  public int ActiveCarrierPickupLocationId { get; set; }
}
public class CreateThirdPartyLocationCommandHandler : RequestHandlerBase<CreateThirdPartyLocationCommand, ServiceResultDTO>
{
  private readonly ICarrierRepository _carrierRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly IConfigRepository _configRepository;
  private readonly ICarrierSharedRepository _carrierSharedRepository;

  public CreateThirdPartyLocationCommandHandler(ICarrierRepository carrierRepository, IOrderRepository orderRepository, IConfigRepository configRepository, ICarrierSharedRepository carrierSharedRepository, IServiceProvider serviceProvider, ILogger<CreateThirdPartyLocationCommandHandler> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
    _orderRepository = orderRepository;
    _configRepository = configRepository;
    _carrierSharedRepository = carrierSharedRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(CreateThirdPartyLocationCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();

    var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.IntegrationKey, _currentUser.EnvironmentTypeId);
    if (mcconfig is null)
    {
      throw new EntityNotFoundException("Mcconfig", "Integration Value");
    }
    var clientId = _currentUser.ClientIdStr!;

    var carreirresult = await _carrierSharedRepository.CreateThirdPartyPickupLocation(request.Weight, request.Quantity, request.ServiceCode,request.ContainerCode, request.ActiveCarrierId,request.carrierid,request.ActiveCarrierPickupLocationId, clientId, mcconfig.Value!);

    if (!string.IsNullOrEmpty(carreirresult))
    {
      var result = JsonConvert.DeserializeObject<CreatePickupLocationResponse>(carreirresult);
      if (result != null && result!.isSuccess)
      {
        serviceResult = new ServiceResultDTO(new BaseResponseDto
        {
          Message = "Pickup Location created successfully",
          Data = result,
        });
      }
      else
      {
        serviceResult.IsSuccess = false;
        serviceResult.Errors = result!.errors?.ToObject<Dictionary<string, string[]>>();
      }
    }
    return serviceResult;
  }
}



