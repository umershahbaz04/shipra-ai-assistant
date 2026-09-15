using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.CarrierUseCase;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.SharedKernel.Interfaces;
using Shipra.Backend.API.SharedKernel.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Shipra.Backend.API.Application.Features.ShipmentFeatures.Query.GetAllCarrierShipmentPodFile;
public class GetAllCarrierShipmentPodFileQuery : IRequest<ServiceResultDTO>
{
  public string? OrderNo { get; set; }
}
public class GetAllCarrierShipmentPodFileQueryHandler : RequestHandlerBase<GetAllCarrierShipmentPodFileQuery, ServiceResultDTO>
{
  private readonly ICarrierSharedRepository _carrierSharedRepository;
  private readonly IConfigRepository _configRepository;
  private readonly ICarrierRepository _carrierRepository;
  private readonly IOrderRepository _orderRepository;

  public GetAllCarrierShipmentPodFileQueryHandler(ICarrierSharedRepository carrierSharedRepository, IConfigRepository configRepository, ICarrierRepository carrierRepository, IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<GetAllCarrierShipmentPodFileQueryHandler> logger) : base(serviceProvider, logger)
  {
    _carrierSharedRepository = carrierSharedRepository;
    _configRepository = configRepository;
    _carrierRepository = carrierRepository;
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllCarrierShipmentPodFileQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      ActiveCarrierContractResponseModel? activeCarrier = null;
      var order = await _orderRepository.GetOrderByOrderNo(request.OrderNo!, _currentUser.ClientId!);
      if (order == null)
      {
        serviceResult.CreateError("Order", new string[] { "Order not found" });
        return serviceResult;
      }
      if (order.CarrierId.GetValueOrDefault() == 0)
      {
        serviceResult.CreateError("OrderCarrier", new string[] { "Carrier not found!" });
        return serviceResult;
      }
      if (order.CarrierContractTypeId == (int)EnumCarrierContractType.ShipraContractType)
      {

        ShipraContractCarrier? shipraContractCarrier = await _carrierRepository.GetShipraContractCarrierByCarrierId(order.CarrierId.GetValueOrDefault());

        if (shipraContractCarrier == null)
        {
          throw new EntityNotFoundException("ShipraContractCarrier", order.CarrierId.GetValueOrDefault());
        }


        ShipraContractClientCarrier? shipraContractClientCarrier = await _carrierRepository.GetShipraContractClientCarrier(_currentUser.ClientId!, shipraContractCarrier.ShipraContractCarrierId);

        if (shipraContractClientCarrier == null)
        {
          throw new EntityNotFoundException("ShipraContractClientCarrier", order.CarrierId.GetValueOrDefault());
        }

        //check if !AllowCodOrder then return with message else continue
        if (!shipraContractClientCarrier.AllowCodOrder.GetValueOrDefault())
        {
          throw new Exception("You cannot assign the order using Shipra client carrier. Please contact with support. ");
        }
        activeCarrier = _mapper.Map<ActiveCarrierContractResponseModel>(shipraContractCarrier);

      }
      else
      {

        ActiveCarrier? oActiveCarrier = await _carrierRepository.GetActiveCarrierByActiveCarrierId(order.ActiveCarrierId.GetValueOrDefault(), _currentUser.ClientId!);
        if (oActiveCarrier == null)
        {
          throw new EntityNotFoundException("ActiveCarrier", order.CarrierId.GetValueOrDefault());
        }
        activeCarrier = _mapper.Map<ActiveCarrierContractResponseModel>(oActiveCarrier);

      }


      var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.IntegrationKey, _currentUser.EnvironmentTypeId);
      if (mcconfig is null)
      {
        throw new EntityNotFoundException("Mcconfig", "Integration Value");
      }

      var clientId = _currentUser.ClientIdStr!;
      var reqModel = new GetCarrierPodFileRequestModel
      {
        ActiveCarrierId = activeCarrier.ActiveCarrierId,
        CarrierId = order.CarrierId.GetValueOrDefault(),
        OrderNo = order.OrderNo,
        TrackingNo = order.CarrierTrackingNo,
      };
      var carreirresult = await _carrierSharedRepository.GetCarrierPodFile(reqModel, _currentUser.ClientIdStr!, mcconfig.Value!);
      IntegrationCarrierResponseModel<OrderData> result = JsonConvert.DeserializeObject<IntegrationCarrierResponseModel<OrderData>>(carreirresult); 
      if (result.isSuccess)
      {
        serviceResult = new ServiceResultDTO(result?.data!);
        // we cannot check issucess value here because now we have both kind of order like success/faild so handle both inside 
      }
      else
      {
        serviceResult.IsSuccess = false;
        serviceResult.Errors= result.errors;
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
#region ShipmentFileResponse 
public class OrderData
{
  public string? OrderNo { get; set; }
  public List<PodModel> PodFiles { get; set; } = new List<PodModel>();

}
public class PodModel
{
  public string? Name { get; set; }
  public List<string> FileUrls { get; set; } = new List<string>();

}
#endregion
