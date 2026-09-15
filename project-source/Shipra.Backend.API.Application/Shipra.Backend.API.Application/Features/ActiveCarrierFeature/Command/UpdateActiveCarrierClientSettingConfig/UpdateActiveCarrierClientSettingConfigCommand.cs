using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.CarrierUseCase;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;
using ShopifySharp;

namespace Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Command.UpdateActiveCarrierClientSettingConfig;
public class UpdateActiveCarrierClientSettingConfigCommand : IRequest<ServiceResultDTO>
{
  public int ActiveCarrierId { get; set; }
  public List<GeneralSettingConfigModel>? SettingConfig { get; set; } = new();
  public int? CarrierId { get; set; }
  public int CarrierContractTypeId { get; set; } = (int)EnumCarrierContractType.OwnContractType;
  public int? ShipraContractCarrierId { get; set; }
  public decimal? FlatRate { get; set; }
  public Dictionary<string, string>? InputParameters { get; set; } 
}
public class UpdateActiveCarrierClientSettingConfigCommandHandler : RequestHandlerBase<UpdateActiveCarrierClientSettingConfigCommand, ServiceResultDTO>
{
  private readonly ICarrierRepository _carrierRepository;

  public UpdateActiveCarrierClientSettingConfigCommandHandler(ICarrierRepository carrierRepository, IServiceProvider serviceProvider, ILogger<UpdateActiveCarrierClientSettingConfigCommandHandler> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdateActiveCarrierClientSettingConfigCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();

    try
    {
      var settingConfig = JsonConvert.SerializeObject(request.SettingConfig!, new JsonSerializerSettings
      {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
      });

      #region MyRegion
      var carrier = await _carrierRepository.GetCarrierFromMasterDbById(request.CarrierId.GetValueOrDefault());
      if (carrier == null)
      {
        throw new EntityNotFoundException("CarrierContract", request.ShipraContractCarrierId.GetValueOrDefault());
      }
      var dict = request.InputParameters! != null && request.InputParameters!.Count > 0 ? Utils.ConvertKeysToCamelCase(request.InputParameters!) : new Dictionary<string, string>();
      var carrierDic = Utils.ConvertKeysToCamelCase(JsonConvert.DeserializeObject<Dictionary<string, string>>(carrier!.Config!)!);
 
      foreach (var entry in dict)
      {
        // do something with entry.Value or entry.Key 
        if (Utils.CheckKeyExistFromDictionryByKey(entry.Key, carrierDic))
        {
          carrierDic[entry.Key] = entry.Value;
        }
      }
      var jsonConfigStr = JsonConvert.SerializeObject(carrierDic, Formatting.Indented);

      #endregion
      if (request.CarrierContractTypeId == (int)EnumCarrierContractType.ShipraContractType)
      {
        ShipraContractCarrier oShipraCarrierContract = await _carrierRepository.GetShipraCarrierContractByCarrierId(request.ShipraContractCarrierId);
        if (oShipraCarrierContract == null)
        {
          throw new EntityNotFoundException("CarrierContract", request.ShipraContractCarrierId.GetValueOrDefault());
        }

        oShipraCarrierContract.UpdateSettingConfig(settingConfig, jsonConfigStr, request.FlatRate);
        var oActiveCarrier = await _carrierRepository.UpdateShipraCarrierContractByCarrierId(oShipraCarrierContract);

        serviceResult.IsSuccess = oActiveCarrier;
        if (serviceResult.IsSuccess)
        {
          serviceResult.CreateSuccessResponse();
        }
      }
      else
      {
        var activeCarrier = await _carrierRepository.GetActiveCarrierByActiveCarrierId(request.ActiveCarrierId, _currentUser.ClientId!);
        if (activeCarrier == null)
        {
          throw new EntityNotFoundException("ActiveCarrier", request.ActiveCarrierId);
        }

        activeCarrier.UpdateSettingConfig(settingConfig, jsonConfigStr, _currentUser.EmployeeId!);
        var oActiveCarrier = await _carrierRepository.UpdateActiveCarrier(activeCarrier);
        if (oActiveCarrier is not null)
        {
          serviceResult.IsSuccess = true;
        }
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

  public class UpdateActiveCarrierClientSettingConfigCommandValidator : AbstractValidator<UpdateActiveCarrierClientSettingConfigCommand>
  {
    public UpdateActiveCarrierClientSettingConfigCommandValidator()
    {  
      When(v => v.CarrierContractTypeId > 0 && v.CarrierContractTypeId == (int)EnumCarrierContractType.OwnContractType, () =>
      {
        RuleFor(x => x.ActiveCarrierId).NotEmpty().NotNull().GreaterThan(0);
      });
      When(v => v.CarrierContractTypeId > 0 && v.CarrierContractTypeId == (int)EnumCarrierContractType.ShipraContractType, () =>
      {
        RuleFor(x => x.FlatRate).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.ShipraContractCarrierId).NotNull().NotEmpty().GreaterThan(0);
      });
    }
  }

}
