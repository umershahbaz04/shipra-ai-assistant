using System.Net;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Application.DTOs.StoresUseCase.Responses;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Core.Helper;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.StoreFeatures.Query.GetStoreByIdQuery;
public class GetStoreByIdQueryHandler : RequestHandlerBase<GetStoreByIdQuery, ServiceResultDTOWithTypeModel<StoreResponseModel>>
{
  private readonly IClientRepository _clientRepository;
  private readonly IStoreRepository _storeRepository;
  public GetStoreByIdQueryHandler(IClientRepository clientRepository, IStoreRepository storeRepository, IServiceProvider serviceProvider, ILogger<GetStoreByIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _clientRepository = clientRepository;
    _storeRepository = storeRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<StoreResponseModel>> HandleRequest(GetStoreByIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTOWithTypeModel<StoreResponseModel> serviceResult = new ServiceResultDTOWithTypeModel<StoreResponseModel>();
    try
    { 
      var target = await _storeRepository.GetStoreById(request.StoreId, _currentUser.ClientId!);
      if (target == null)
      {
        throw new EntityNotFoundException("Store ", request.StoreId);
      }

      var oStoreAddress = await _storeRepository.GetStoreAddressById(target.StoreId);
      if (oStoreAddress is null)
      {
        throw new EntityNotFoundException("Store Address ", request.StoreId);
      }
      var model = _mapper.Map<StoreResponseModel>(target);
      
      var oAddressMap = _mapper.Map<AddressResponseDTO>(oStoreAddress);
      model.Address = oAddressMap;

      serviceResult = new ServiceResultDTOWithTypeModel<StoreResponseModel>(model);
      serviceResult.CreateSuccessResponse(HttpStatusCode.OK);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }


}
