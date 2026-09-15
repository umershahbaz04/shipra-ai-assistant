using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.CarrierUseCase;
using Shipra.Backend.API.Application.MediatorNotification;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.WalletAggregate;
using Shipra.Backend.API.SharedKernel.Interfaces;
using Shipra.Backend.API.SharedKernel.Models;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetCalculatedRateByCarrier;
public class GetCalculatedRateByCarrierQuery : ThirdPartyRateRequestModel,IRequest<ServiceResultDTO>
{
  public int? ShipraContractCarrierId { get; set; } = 0; // in case of future use
  public bool? CheckPikupLocation { get; set; }
  public string? ServiceType { get; set; } 
}
public class GetCalculatedRateByCarrierQueryHandler : RequestHandlerBase<GetCalculatedRateByCarrierQuery, ServiceResultDTO>
{
  private readonly ICarrierSharedRepository _carrierSharedRepository;
  private readonly IConfigRepository _configRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly ICarrierRepository _carrierRepository;
  private readonly IClientRepository _clientRepository;

  public GetCalculatedRateByCarrierQueryHandler(ICarrierSharedRepository carrierSharedRepository,IConfigRepository configRepository,IOrderRepository orderRepository,ICarrierRepository carrierRepository,IClientRepository clientRepository,IServiceProvider serviceProvider, ILogger<GetCalculatedRateByCarrierQueryHandler> logger) : base(serviceProvider, logger)
  {
    _carrierSharedRepository = carrierSharedRepository;
    _configRepository = configRepository;
    _orderRepository = orderRepository;
    _carrierRepository = carrierRepository;
    _clientRepository = clientRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetCalculatedRateByCarrierQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var client = await _clientRepository.GetClientById(_currentUser.ClientId!);
      if (client is null)
      {
        throw new EntityNotFoundException("Client ", _currentUser.ClientIdStr!.ToString());
      }

      ActiveCarrierContractResponseModel? activeCarrier = null;
      if (request.CarrierContractTypeId == (int)EnumCarrierContractType.ShipraContractType)
      {  
        ShipraContractCarrier? shipraContractCarrier = await _carrierRepository.GetShipraContractCarrierByCarrierId(request.CarrierId);

        if (shipraContractCarrier == null)
        {
          throw new EntityNotFoundException("ShipraContractCarrier", request.CarrierId);
        } 
        ShipraContractClientCarrier? shipraContractClientCarrier = await _carrierRepository.GetShipraContractClientCarrier(_currentUser.ClientId!, shipraContractCarrier.ShipraContractCarrierId);

        if (shipraContractClientCarrier == null)
        {
          throw new EntityNotFoundException("ShipraContractClientCarrier", request.CarrierId);
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

        ActiveCarrier? oActiveCarrier = await _carrierRepository.GetActiveCarrierByActiveCarrierId(request.ActiveCarrierId, _currentUser.ClientId!);
        if (oActiveCarrier == null)
        {
          throw new EntityNotFoundException("ActiveCarrier", request.CarrierId);
        }
        activeCarrier = _mapper.Map<ActiveCarrierContractResponseModel>(oActiveCarrier);

      }
      string? orderNos = request.OrderNos;
      var orderList = await _orderRepository.GetOrdersWithOrderNos(orderNos, _currentUser.ClientId!);
      //check if order type fullfilable then order must be fullfilled 
      var unFullfilledeOrdersCheck = orderList.Where(x => x.OrderTypeId == (int)EnumOrderType.FullFilable && x.FullFillmentStatusId != (int)EnumFullfillmentStatus.Fulfilled).ToList();
      var lockedOrdersCheck = orderList.Where(x => x.TrackingLock == true).ToList();
      if (lockedOrdersCheck.Count > 0)
      {
        serviceResult.IsSuccess = false;
        serviceResult.StatusCode = (int)HttpStatusCode.Conflict;
        serviceResult.Errors!.Add("OrdersLocked", new string[] { $"you cannot assign locked order,Please remove the given order(s) no. before proceed " + string.Join(',', lockedOrdersCheck.Select(x => x.OrderNo).ToList()) });

        return serviceResult;
      }
      if (unFullfilledeOrdersCheck.Count > 0)
      {
        serviceResult.IsSuccess = false;
        serviceResult.StatusCode = (int)HttpStatusCode.Conflict;
        serviceResult.Errors!.Add("OrdersNotFulfilled", new string[] { $"Please fulfilled the given order(s) no. before proceed " + string.Join(',', unFullfilledeOrdersCheck.Select(x => x.OrderNo).ToList()) });

        return serviceResult;
      }

      //checking if order already assign to carrier 
      var existCarrier = orderList.Where(x => x.CarrierId > 0 && x.CarrierId != null).ToList();
      if (existCarrier.Count() == 0)
      {
        var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.IntegrationKey, _currentUser.EnvironmentTypeId);
        if (mcconfig is null)
        {
          throw new EntityNotFoundException("Mcconfig", "Integration Value");
        }

         
        var clientId = _currentUser.ClientIdStr!; 
        var carreirresult = await _carrierSharedRepository.GetCalcuatedRate(activeCarrier.CarrierId, activeCarrier.ActiveCarrierId, clientId, orderNos, request.orderList!, request.IsOther,request.CarrierContractTypeId, mcconfig.Value!);

        if (!string.IsNullOrEmpty(carreirresult))
        {
          IntegrationCarrierResponseModel<OrderWithRates> result = JsonConvert.DeserializeObject<IntegrationCarrierResponseModel<OrderWithRates>>(carreirresult);
          // we cannot check issucess value here because now we have both kind of order like success/faild so handle both inside
          if (result != null)
          {
            if (result!.data! != null)
            {
              var updatedOrderWithRates = new OrderWithRates
              {
                OrderNo = result!.data.OrderNo,
                RateResult = result!.data.RateResult!.Select(x =>
                {
                  if(x.Skip)
                  {
                    x.Text = $"{x.Amount} {x.Currency}";
                  }
                  else
                  {
                    x.Text = $"{x.ProductType} - {x.Amount} {x.Currency}";
                  }
                  return x;
                }).ToList()
              };
              serviceResult = new ServiceResultDTO(updatedOrderWithRates);
            }
            if (result.errors != null || result.configErrors != null)
            {
              serviceResult.IsSuccess = false;
              serviceResult.Errors = result!.errors;
              serviceResult.ConfigErrors = result!.configErrors;
            }
          }
          else
          {
            serviceResult.IsSuccess = false;
            serviceResult.Errors = result!.errors;
            serviceResult.ConfigErrors = result!.configErrors;
          }
        }
        else
        {
          serviceResult.IsSuccess = false;
          serviceResult.Errors!.Add("ThirdPartyError", new string[] { "Error while placing order" });
        }

      }
      else
      {
        serviceResult.IsSuccess = false;
        serviceResult.StatusCode = (int)HttpStatusCode.Conflict;
        serviceResult.Errors!.Add("AlreadyAssigned", new string[] { "Following orders are already assigned. " + string.Join(',', existCarrier.Select(x => x.OrderNo)) });
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
