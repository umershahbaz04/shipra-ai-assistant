using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.Helper;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CarrierFeatures.Query.GetInputRequiredConfigById;
public class GetInputRequiredConfigByIdQuery : IRequest<ServiceResultDTO>
{
  public int CarrierId { get; set; }
}
public class GetInputRequiredConfigByIdQueryHandler : RequestHandlerBase<GetInputRequiredConfigByIdQuery, ServiceResultDTO>
{
  private readonly IStoreRepository _storeRepository;
  private readonly IClientRepository _clientRepository;
  private readonly ICarrierRepository _carrierRepository;

  public GetInputRequiredConfigByIdQueryHandler(IStoreRepository storeRepository, IClientRepository clientRepository, ICarrierRepository carrierRepository, IServiceProvider serviceProvider, ILogger<GetInputRequiredConfigByIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _storeRepository = storeRepository;
    _clientRepository = clientRepository;
    _carrierRepository = carrierRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetInputRequiredConfigByIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var carrier = await _carrierRepository.GetCarrierFromMasterDbById(request.CarrierId);
      if (carrier is null)
      {
        throw new EntityNotFoundException("Carrier", request.CarrierId);
      }

      List<ActiveCarrier> oActiveCarrierAlias = await _carrierRepository.GetActiveCarrieriersByCarrierId(request.CarrierId, _currentUser.ClientId);
      int count = oActiveCarrierAlias.Count + 1;
      var name = carrier.Name + " " + count;

      #region get store address 
      if (!string.IsNullOrEmpty(carrier.SettingConfig!))
      {
        var client = await _clientRepository.GetClientById(_currentUser.ClientId!);
        if (client is not null)
        {
          var oStore = await _storeRepository.GetStoreById(client.DefaultStoreId.GetValueOrDefault(), _currentUser.ClientId!);
          if (oStore is not null)
          {
            var oAddress = await _storeRepository.GetStoreAddressById(oStore.StoreId);

            if (oAddress is not null)
            {

              // Call the function to update the JSON
              string updatedJsonString = UtilityHelper.UpdateJsonCarrierSettingConfigValue(carrier.SettingConfig!, "store", "HouseNo", oAddress!.HouseNo!);
              updatedJsonString = UtilityHelper.UpdateJsonCarrierSettingConfigValue(updatedJsonString, "store", "BuildingName", oAddress!.BuildingName!);
              updatedJsonString = UtilityHelper.UpdateJsonCarrierSettingConfigValue(updatedJsonString, "store", "Landmark", oAddress!.Landmark!);

              carrier.SettingConfig = updatedJsonString;
            }
          }
        }
      }
      #endregion

      serviceResult = new ServiceResultDTO(new { InputRequiredConfig = carrier.InputRequiredConfig, nextAliasName = name, settingConfig = carrier.SettingConfig });

      return serviceResult;
    }
    catch (Exception)
    {

      throw;
    }
  }
}
