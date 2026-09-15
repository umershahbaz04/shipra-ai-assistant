using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs.StoresUseCase.Responses;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.StoreFeatures.Query.GetStoreByIdQuery;
using Shipra.Backend.API.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using System.Net;
using MediatR;
using Shipra.Backend.API.Application.DTOs.ActiveCarrierPickupLoctionUseCase;
using System.Runtime.InteropServices.JavaScript;
using Nancy.Json;
using Newtonsoft.Json;
using Shipra.Backend.API.Core.Helper;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.CarrierAggregate;

namespace Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Query.GetActiveCarrierPickupLocationbyId;

public class GetActiveCarrierPickupLocationbyIdQueryHandler : RequestHandlerBase<GetActiveCarrierLocationbyIdQuery, ServiceResultDTO>
{
  private readonly IClientRepository _clientRepository;
  private readonly IStoreRepository _storeRepository;
  private readonly ICarrierRepository _carrierRepository;
  public GetActiveCarrierPickupLocationbyIdQueryHandler(IClientRepository clientRepository, ICarrierRepository carrierRepository, IStoreRepository storeRepository, IServiceProvider serviceProvider, ILogger<GetActiveCarrierPickupLocationbyIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _clientRepository = clientRepository;
    _storeRepository = storeRepository;
    _carrierRepository = carrierRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(GetActiveCarrierLocationbyIdQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var ActiveCarrierPickupLocation = await _carrierRepository.GetActiveCarrierLocationbyId(request.ActiveCarrierPickupLocationId, _currentUser.ClientId!);
      var data = JsonConvert.SerializeObject(ActiveCarrierPickupLocation);
      var entityMappings = CivilEntityHelper.GetEntityMappings(ActiveCarrierPickupLocation!.EntityAddressDataJson);
      // Ensure default values when JSON is null or does not contain a mapping
      if (string.IsNullOrEmpty(ActiveCarrierPickupLocation!.EntityAddressDataJson))
      {
        EnsureDefaultEntityMappings(entityMappings, ActiveCarrierPickupLocation);
      }
      if (ActiveCarrierPickupLocation == null)
      {
        throw new EntityNotFoundException("ActiveCarrierLocation ", request.ActiveCarrierPickupLocationId);
      }
      var model = _mapper.Map<PickupLocationResponseModel>(ActiveCarrierPickupLocation);
      if (model == null)
      {
        throw new EntityNotFoundException("ActiveCarrierLocation ", "Something went wrong while convert model");
      }
      var carrier = await _carrierRepository.GetCarrierById(model.CarrierId);
      model.IsDispatchExCompany = carrier?.IsDispatchExCompany;
      #region Override
      model.address!.city = CivilEntityHelper.GetEntityValue(entityMappings, EnumCivilEntityType.City);
      model.address.area = CivilEntityHelper.GetEntityValue(entityMappings, EnumCivilEntityType.Area);
      model.address.province = CivilEntityHelper.GetEntityValue(entityMappings, EnumCivilEntityType.Province);
      model.address.state = CivilEntityHelper.GetEntityValue(entityMappings, EnumCivilEntityType.State);
      model.address.pinCode = CivilEntityHelper.GetEntityValue(entityMappings, EnumCivilEntityType.PinCode);
      #endregion
      serviceResult = new ServiceResultDTO(model);
      serviceResult.CreateSuccessResponse(HttpStatusCode.OK);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
  // Ensures default values when JSON is null
  private static void EnsureDefaultEntityMappings(Dictionary<EnumCivilEntityType, string> mappings, ActiveCarrierPickupLocation orderAddress)
  {
    if (!mappings.ContainsKey(EnumCivilEntityType.City) && orderAddress.CityId.HasValue && orderAddress.CityId > 0)
      mappings[EnumCivilEntityType.City] = $"{orderAddress.CityId}_0";

    if (!mappings.ContainsKey(EnumCivilEntityType.Area) && orderAddress.AreaId.HasValue && orderAddress.AreaId > 0)
      mappings[EnumCivilEntityType.Area] = $"{orderAddress.AreaId}_0";

    if (!mappings.ContainsKey(EnumCivilEntityType.Province) && orderAddress.ProvinceId.HasValue && orderAddress.ProvinceId > 0)
      mappings[EnumCivilEntityType.Province] = $"{orderAddress.ProvinceId}_0";

    if (!mappings.ContainsKey(EnumCivilEntityType.State) && orderAddress.StateId.HasValue && orderAddress.StateId > 0)
      mappings[EnumCivilEntityType.State] = $"{orderAddress.StateId}_0";

    if (!mappings.ContainsKey(EnumCivilEntityType.PinCode) && orderAddress.PinCodeId.HasValue && orderAddress.PinCodeId > 0)
      mappings[EnumCivilEntityType.PinCode] = $"{orderAddress.PinCodeId}_0";
  }
  // Retrieves entity value from mappings
}
