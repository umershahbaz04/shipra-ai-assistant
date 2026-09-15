using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.CarrierUseCase;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.SharedKernel.Interfaces;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.RefreshCarrierStatus;
public class RefreshCarrierStatusCommand : IRequest<ServiceResultDTO>
{
  public string? OrderId { get; set; }
  public string? OrderNo { get; set; }
}
public class RefreshCarrierStatusCommandHandler : RequestHandlerBase<RefreshCarrierStatusCommand, ServiceResultDTO>
{
  private readonly ICarrierRepository _carrierRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly IConfigRepository _configRepository;
  private readonly ICarrierSharedRepository _carrierSharedRepository;

  public RefreshCarrierStatusCommandHandler(ICarrierRepository carrierRepository, IOrderRepository orderRepository, IConfigRepository configRepository, ICarrierSharedRepository carrierSharedRepository, IServiceProvider serviceProvider, ILogger<RefreshCarrierStatusCommandHandler> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
    _orderRepository = orderRepository;
    _configRepository = configRepository;
    _carrierSharedRepository = carrierSharedRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(RefreshCarrierStatusCommand request, CancellationToken cancellationToken)
  {
    LogHelper.Write($"[START] HandleRequest called with OrderId: {request.OrderId}, OrderNo: {request.OrderNo}, ClientId: {_currentUser.ClientId}");

    try
    {
      var serviceResult = new ServiceResultDTO();
      Order? oOrder = null;

      if (!string.IsNullOrEmpty(request.OrderId))
      {
        LogHelper.Write($"Fetching order by OrderId: {request.OrderId}");
        oOrder = await _orderRepository.GetOrderById(new OrderId(new Guid(request.OrderId!)), _currentUser.ClientId!);
      }
      else if (!string.IsNullOrEmpty(request.OrderNo))
      {
        LogHelper.Write($"Fetching order by OrderNo: {request.OrderNo}");
        oOrder = await _orderRepository.GetOrderByOrderNo(request.OrderNo, _currentUser.ClientId!);
      }

      if (oOrder is null)
      {
        LogHelper.Write($"[ERROR] Order not found. OrderId: {request.OrderId}, OrderNo: {request.OrderNo}");
        throw new EntityNotFoundException("Order", request.OrderId!);
      }

      LogHelper.Write($"Order found. OrderId: {oOrder.OrderId}, OrderNo: {oOrder.OrderNo}, CarrierId: {oOrder.CarrierId}, TrackingLock: {oOrder.TrackingLock}");

      if (!oOrder.TrackingLock.GetValueOrDefault())
      {
        if (oOrder.CarrierId != null)
        {
          ActiveCarrierContractResponseModel? activeCarrier = null;

          if (oOrder.CarrierContractTypeId == (int)EnumCarrierContractType.ShipraContractType)
          {
            LogHelper.Write($"Fetching ShipraContractCarrier for CarrierId: {oOrder.CarrierId}, OrderNo: {oOrder.OrderNo}");
            ShipraContractCarrier? shipraContractCarrier =
                await _carrierRepository.GetShipraContractCarrierByCarrierId(oOrder.CarrierId.GetValueOrDefault());

            if (shipraContractCarrier == null)
            {
              LogHelper.Write($"[ERROR] ShipraContractCarrier not found. CarrierId: {oOrder.CarrierId}, OrderNo: {oOrder.OrderNo}");
              throw new EntityNotFoundException("ShipraContractCarrier", oOrder.CarrierId);
            }

            activeCarrier = _mapper.Map<ActiveCarrierContractResponseModel>(shipraContractCarrier);
            LogHelper.Write($"Mapped ShipraContractCarrier to ActiveCarrierContractResponseModel. CarrierId: {oOrder.CarrierId}, OrderNo: {oOrder.OrderNo}");
          }
          else
          {
            LogHelper.Write($"Fetching ActiveCarrier for ActiveCarrierId: {oOrder.ActiveCarrierId}, OrderNo: {oOrder.OrderNo}");
            ActiveCarrier? oActiveCarrier =
                await _carrierRepository.GetActiveCarrierByActiveCarrierId(oOrder.ActiveCarrierId.GetValueOrDefault(), _currentUser.ClientId!);

            if (oActiveCarrier == null)
            {
              LogHelper.Write($"[ERROR] ActiveCarrier not found. ActiveCarrierId: {oOrder.ActiveCarrierId}, OrderNo: {oOrder.OrderNo}");
              throw new EntityNotFoundException("ActiveCarrier", oOrder.CarrierId);
            }

            activeCarrier = _mapper.Map<ActiveCarrierContractResponseModel>(oActiveCarrier);
            LogHelper.Write($"Mapped ActiveCarrier to ActiveCarrierContractResponseModel. ActiveCarrierId: {oOrder.ActiveCarrierId}, OrderNo: {oOrder.OrderNo}");
          }

          if (!activeCarrier.IsDefault.GetValueOrDefault())
          {
            LogHelper.Write($"Fetching integration config (Mcconfig) for OrderNo: {oOrder.OrderNo}");
            var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.IntegrationKey, _currentUser.EnvironmentTypeId);

            if (mcconfig is null)
            {
              LogHelper.Write($"[ERROR] Mcconfig not found for IntegrationKey. OrderNo: {oOrder.OrderNo}");
              throw new EntityNotFoundException("Mcconfig", "Integration Value");
            }

            var clientId = _currentUser.ClientIdStr!;
            LogHelper.Write($"Calling RefreshCarrierStatus. OrderId: {oOrder.OrderId}, OrderNo: {oOrder.OrderNo}, CarrierId: {oOrder.CarrierId}, ActiveCarrierId: {oOrder.ActiveCarrierId}, ClientId: {clientId}");

            var carreirresult = await _carrierSharedRepository.RefreshCarrierStatus(
                request.OrderId,
                oOrder.CarrierId.GetValueOrDefault(),
                oOrder.ActiveCarrierId.GetValueOrDefault(),
                clientId,
                mcconfig.Value!);

            if (!string.IsNullOrEmpty(carreirresult))
            {
              LogHelper.Write($"Carrier response received for OrderNo: {oOrder.OrderNo} → {carreirresult}");
              IntegrationCarrierResponseModel<RefreshStatusResponse> result =
                  JsonConvert.DeserializeObject<IntegrationCarrierResponseModel<RefreshStatusResponse>>(carreirresult);

              if (result != null && result!.isSuccess)
              {
                LogHelper.Write($"Carrier status refresh succeeded for OrderNo: {oOrder.OrderNo}");
                serviceResult = new ServiceResultDTO(new BaseResponseDto
                {
                  Message = "Status update successfully",
                  Data = result!.data,
                });
              }
              else
              {
                LogHelper.Write($"[ERROR] Carrier response indicates failure. OrderNo: {oOrder.OrderNo}");
              }
            }
            else
            {
              LogHelper.Write($"[ERROR] Empty carrier response received. OrderNo: {oOrder.OrderNo}");
              serviceResult.IsSuccess = false;
              serviceResult.Errors!.Add("ThirdPartyError", new string[] { "Error while refresh" });
            }
          }
        }
      }
      else
      {
        LogHelper.Write($"[INFO] Order tracking is locked. OrderNo: {oOrder.OrderNo}");
        serviceResult!.CreateError("OrderLocked", new string[] { "Order tracking is locked." });
      }

      LogHelper.Write($"[END] HandleRequest completed successfully. OrderNo: {oOrder.OrderNo}");
      return serviceResult;
    }
    catch (Exception ex)
    {
      LogHelper.Write($"[EXCEPTION] {ex.GetType().Name}: {ex.Message}, OrderId: {request.OrderId}, OrderNo: {request.OrderNo}");
      throw;
    }
  }

}
