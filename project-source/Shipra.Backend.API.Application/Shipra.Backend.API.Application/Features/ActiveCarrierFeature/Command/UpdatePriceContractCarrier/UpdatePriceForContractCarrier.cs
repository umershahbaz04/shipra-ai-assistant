using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Command.UpdateActiveCarrierClientSettingConfig;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Command.UpdatePriceContractCarrier;
public class UpdatePriceForContractCarrierCommand : IRequest<ServiceResultDTO>
{
  public decimal? FlatRate { get; set; }
  public int? ShipraContractCarrierId { get; set; }
}
public class UpdatePriceForContractCarrierCommandHandler : RequestHandlerBase<UpdatePriceForContractCarrierCommand, ServiceResultDTO>
{
  private readonly ICarrierRepository _carrierRepository;

  public UpdatePriceForContractCarrierCommandHandler(ICarrierRepository carrierRepository, IServiceProvider serviceProvider, ILogger<UpdateActiveCarrierClientSettingConfigCommandHandler> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdatePriceForContractCarrierCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      #region MyRegion
        ShipraContractCarrier oShipraCarrierContract = await _carrierRepository.GetShipraCarrierContractByCarrierId(request.ShipraContractCarrierId);
        if (oShipraCarrierContract == null)
        {
          throw new EntityNotFoundException("CarrierContract", request.ShipraContractCarrierId.GetValueOrDefault());
        }

        oShipraCarrierContract.UpdateCarrierPrice(request.FlatRate);
        var oActiveCarrier = await _carrierRepository.UpdateShipraCarrierContractByCarrierId(oShipraCarrierContract);

        serviceResult.IsSuccess = oActiveCarrier;
        if (serviceResult.IsSuccess)
        {
          serviceResult.CreateSuccessResponse();
        }
     
      else
      {
        serviceResult.CreateSuccessResponse();
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
#endregion
